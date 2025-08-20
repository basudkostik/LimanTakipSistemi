import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Search, Anchor } from 'lucide-react';
import { portAPI } from '../../services/api';
import { Port, AddPortRequest, UpdatePortRequest } from '../../types';
import MainLayout from '../../components/Layout/MainLayout';
import PortForm from './PortForm';

const PortList: React.FC = () => {
  const [ports, setPorts] = useState<Port[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingPort, setEditingPort] = useState<Port | null>(null);
  const [filterCountry, setFilterCountry] = useState('');

  useEffect(() => {
    loadPorts();
  }, []);

  const loadPorts = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await portAPI.getAll();
      console.log('Ports loaded:', response.data);
      setPorts(response.data || []);
    } catch (error) {
      console.error('Error loading ports:', error);
      setError('Limanlar yüklenirken hata oluştu');
      setPorts([]);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: AddPortRequest) => {
    try {
      await portAPI.create(data);
      setShowForm(false);
      loadPorts();
    } catch (error) {
      console.error('Error creating port:', error);
    }
  };

  const handleUpdate = async (id: number, data: UpdatePortRequest) => {
    try {
      await portAPI.update(id, data);
      setEditingPort(null);
      loadPorts();
    } catch (error) {
      console.error('Error updating port:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Bu limanı silmek istediğinizden emin misiniz?')) {
      try {
        await portAPI.delete(id);
        loadPorts();
      } catch (error) {
        console.error('Error deleting port:', error);
      }
    }
  };

  const filteredPorts = ports.filter(port => {
    const matchesSearch = port.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         port.city?.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         port.country?.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCountry = !filterCountry || port.country === filterCountry;
    
    return matchesSearch && matchesCountry;
  });

  const uniqueCountries = [...new Set(ports.map(port => port.country).filter(Boolean))];

  const PortFilters = (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Arama
        </label>
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="Liman adı, şehir veya ülke..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="input-field pl-10"
          />
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Ülke
        </label>
        <select
          value={filterCountry}
          onChange={(e) => setFilterCountry(e.target.value)}
          className="input-field"
        >
          <option value="">Tüm Ülkeler</option>
          {uniqueCountries.map(country => (
            <option key={country} value={country}>{country}</option>
          ))}
        </select>
      </div>
    </div>
  );

  if (loading) {
    return (
      <MainLayout sidebarContent={PortFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Limanlar yükleniyor...</p>
          </div>
        </div>
      </MainLayout>
    );
  }

  if (error) {
    return (
      <MainLayout sidebarContent={PortFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="text-red-600 text-xl mb-4">⚠️</div>
            <p className="text-red-600 mb-4">{error}</p>
            <button 
              onClick={loadPorts}
              className="btn-primary"
            >
              Tekrar Dene
            </button>
          </div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout sidebarContent={PortFilters}>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Liman Yönetimi</h1>
            <p className="text-gray-600">Limanları listele, ekle, düzenle ve sil</p>
          </div>
          <button
            onClick={() => setShowForm(true)}
            className="btn-primary flex items-center space-x-2"
          >
            <Plus className="h-4 w-4" />
            <span>Yeni Liman Ekle</span>
          </button>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Toplam Liman</div>
            <div className="text-2xl font-bold text-gray-900">{ports.length}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Farklı Ülke</div>
            <div className="text-2xl font-bold text-gray-900">{uniqueCountries.length}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Filtrelenen</div>
            <div className="text-2xl font-bold text-gray-900">{filteredPorts.length}</div>
          </div>
        </div>

        {/* Ports Table */}
        <div className="card">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="table-header">Liman Adı</th>
                  <th className="table-header">Ülke</th>
                  <th className="table-header">Şehir</th>
                  <th className="table-header">İşlemler</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredPorts.length === 0 ? (
                  <tr>
                    <td colSpan={4} className="table-cell text-center text-gray-500 py-8">
                      {searchTerm || filterCountry ? 'Arama kriterlerine uygun liman bulunamadı' : 'Henüz liman eklenmemiş'}
                    </td>
                  </tr>
                ) : (
                  filteredPorts.map((port) => (
                    <tr key={port.portId} className="hover:bg-gray-50">
                      <td className="table-cell font-medium">{port.name || 'İsimsiz'}</td>
                      <td className="table-cell">{port.country || 'Belirtilmemiş'}</td>
                      <td className="table-cell">{port.city || 'Belirtilmemiş'}</td>
                      <td className="table-cell">
                        <div className="flex space-x-2">
                          <button
                            onClick={() => setEditingPort(port)}
                            className="text-blue-600 hover:text-blue-800 p-1"
                            title="Düzenle"
                          >
                            <Edit className="h-4 w-4" />
                          </button>
                          <button
                            onClick={() => handleDelete(port.portId)}
                            className="text-red-600 hover:text-red-800 p-1"
                            title="Sil"
                          >
                            <Trash2 className="h-4 w-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* Create/Edit Form Modal */}
      {(showForm || editingPort) && (
        <PortForm
          port={editingPort}
          onSubmit={editingPort ? handleUpdate : handleCreate}
          onCancel={() => {
            setShowForm(false);
            setEditingPort(null);
          }}
        />
      )}
    </MainLayout>
  );
};

export default PortList; 