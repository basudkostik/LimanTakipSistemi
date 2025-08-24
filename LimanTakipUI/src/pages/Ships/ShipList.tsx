import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Eye, Search, Filter } from 'lucide-react';
import { shipAPI } from '../../services/api';
import { Ship, AddShipRequest, UpdateShipRequest } from '../../types';
import MainLayout from '../../components/Layout/MainLayout';
import ShipForm from './ShipForm';

const ShipList: React.FC = () => {
  const [ships, setShips] = useState<Ship[]>([]);
  const [allShips, setAllShips] = useState<Ship[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filterName, setFilterName] = useState('');
  const [filterIMO, setFilterIMO] = useState('');
  const [filterYearBuilt, setFilterYearBuilt] = useState('');
  const [filterType, setFilterType] = useState('');
  const [filterFlag, setFilterFlag] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingShip, setEditingShip] = useState<Ship | null>(null);
 
  const shipTypes = [
    'Konteyner Gemisi',
    'Tanker',
    'Bulk Carrier',
    'Ro-Ro Gemisi',
    'Yolcu Gemisi',
    'Balıkçı Gemisi',
    'Yat',
    'Diğer'
  ];

  useEffect(() => {
    loadShips();
  }, [filterName, filterIMO, filterYearBuilt, filterType, filterFlag ]);

  const params = {
    name: filterName || undefined,
    IMO: filterIMO || undefined,
    yearbuilt: filterYearBuilt || undefined,
    type: filterType || undefined,
    flag: filterFlag || undefined,
    pageNumber: 1,
    pageSize: 100
  };

  const loadShips = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await shipAPI.getAll(params);
      const response_all = await shipAPI.getAll();
      setAllShips(response_all.data || []);
      console.log('Ships loaded:', response.data);
      setShips(response.data || []);
    } catch (error) {
      console.error('Error loading ships:', error);
      setError('Gemiler yüklenirken hata oluştu');
      setShips([]);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: AddShipRequest) => {
    try {
      await shipAPI.create(data);
      setShowForm(false);
      loadShips();
      console.log("🚀 Creating ship with data:", data);
    } catch (error) {
      console.log("🚀 Creating ship with data:", data);
      console.error('Error creating ship:', error);
      throw error; // Hatayı form'a geri gönder
    }
  };

  const handleUpdate = async (id: number, data: UpdateShipRequest) => {
    try {
      await shipAPI.update(id, data);
      setEditingShip(null);
      loadShips();
    } catch (error) {
      console.error('Error updating ship:', error);
      throw error; // Hatayı form'a geri gönder
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Bu gemiyi silmek istediğinizden emin misiniz?')) {
      try {
        await shipAPI.delete(id);
        loadShips();
      } catch (error) {
        console.error('Error deleting ship:', error);
      }
    }
  };

   
  
  const uniqueTypes = [...new Set(allShips.map(ship => ship.type).filter(Boolean))];
  const uniqueFlags = [...new Set(allShips.map(ship => ship.flag).filter(Boolean))];

  // Safe calculations with null checks
  const totalYearBuilt = ships.reduce((sum, ship) => {
    const yearBuilt = ship.yearBuilt || 0;
    return sum + yearBuilt;
  }, 0);

  const ShipFilters = (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Gemi Adı
        </label>
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="Gemi adını giriniz..."
            value={filterName}
            onChange={(e) => setFilterName(e.target.value)}
            className="input-field pl-10"
          />
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          IMO Numarası
        </label>
        <input
          type="text"
          placeholder="IMO numarasını giriniz..."
          value={filterIMO}
          onChange={(e) => setFilterIMO(e.target.value)}
          className="input-field"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Yapım Yılı
        </label>
        <input
          type="number"
          placeholder="Yapım yılını giriniz..."
          value={filterYearBuilt}
          onChange={(e) => setFilterYearBuilt(e.target.value)}
          min="1900"
          max={new Date().getFullYear() + 1}
          className="input-field"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Gemi Tipi
        </label>
        <select
          value={filterType}
          onChange={(e) => setFilterType(e.target.value)}
          className="input-field"
        >
          <option value="">Tüm Tipler</option>
          {shipTypes.map(type => (
            <option key={type} value={type}>{type}</option>
          ))}
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Bayrak
        </label>
        <select
          value={filterFlag}
          onChange={(e) => setFilterFlag(e.target.value)}
          className="input-field"
        >
          <option value="">Tüm Bayraklar</option>
          {uniqueFlags.map(flag => (
            <option key={flag} value={flag}>{flag}</option>
          ))}
        </select>
      </div>

      {/* Filtreleri Temizle Butonu */}
      <div className="pt-2">
        <button
          onClick={() => {
            setFilterName('');
            setFilterIMO('');
            setFilterYearBuilt('');
            setFilterType('');
            setFilterFlag('');
          }}
          className="w-full btn-secondary text-sm py-2"
        >
          Filtreleri Temizle
        </button>
      </div>
    </div>
  );

  if (loading) {
    return (
      <MainLayout sidebarContent={ShipFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Gemiler yükleniyor...</p>
          </div>
        </div>
      </MainLayout>
    );
  }

  if (error) {
    return (
      <MainLayout sidebarContent={ShipFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="text-red-600 text-xl mb-4">⚠️</div>
            <p className="text-red-600 mb-4">{error}</p>
            <button 
              onClick={loadShips}
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
    <MainLayout sidebarContent={ShipFilters}>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Gemi Yönetimi</h1>
            <p className="text-gray-600">Gemileri listele, ekle, düzenle ve sil</p>
          </div>
          <button
            onClick={() => setShowForm(true)}
            className="btn-primary flex items-center space-x-2"
          >
            <Plus className="h-4 w-4" />
            <span>Yeni Gemi Ekle</span>
          </button>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Toplam Gemi</div>
            <div className="text-2xl font-bold text-gray-900">{allShips.length}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Farklı Tip</div>
            <div className="text-2xl font-bold text-gray-900">{uniqueTypes.length}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Ortalama Yapım Yılı</div>
            <div className="text-2xl font-bold text-gray-900">
              {ships.length > 0 ? Math.round(totalYearBuilt / ships.length) : 0}
            </div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Farklı Bayrak</div>
            <div className="text-2xl font-bold text-gray-900">{uniqueFlags.length}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Filtrelenen</div>
            <div className="text-2xl font-bold text-gray-900">{ships.length}</div>
          </div>
        </div>

        {/* Ships Table */}
        <div className="card">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="table-header">Gemi Adı</th>
                  <th className="table-header">IMO</th>
                  <th className="table-header">Tip</th>
                  <th className="table-header">Bayrak</th>
                  <th className="table-header">Yapım Yılı</th>
                  <th className="table-header">İşlemler</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {ships.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="table-cell text-center text-gray-500 py-8">
                      {filterName || filterType || filterFlag ? 'Arama kriterlerine uygun gemi bulunamadı' : 'Henüz gemi eklenmemiş'}
                    </td>
                  </tr>
                ) : (
                  ships.map((ship) => (
                    <tr key={ship.shipId} className="hover:bg-gray-50">
                      <td className="table-cell font-medium">{ship.name || 'İsimsiz'}</td>
                      <td className="table-cell font-mono">{ship.imo || 'Belirtilmemiş'}</td>
                      <td className="table-cell">{ship.type || 'Belirtilmemiş'}</td>
                      <td className="table-cell">{ship.flag || 'Belirtilmemiş'}</td>
                      <td className="table-cell">{ship.yearBuilt || 'Belirtilmemiş'}</td>
                      <td className="table-cell">
                        <div className="flex space-x-2">
                          <button
                            onClick={() => setEditingShip(ship)}
                            className="text-blue-600 hover:text-blue-800 p-1"
                            title="Düzenle"
                          >
                            <Edit className="h-4 w-4" />
                          </button>
                          <button
                            onClick={() => handleDelete(ship.shipId)}
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
      {(showForm || editingShip) && (
        <ShipForm
          ship={editingShip}
          onSubmit={async (data) => {
                if (editingShip)
                    {
                        await handleUpdate(editingShip.shipId, data); 
                    }
                     else
                       {
      await handleCreate(data); 
    }
  }}
          onCancel={() => {
            setShowForm(false);
            setEditingShip(null);
          }}
        />
      )}
    </MainLayout>
  );
};

export default ShipList; 