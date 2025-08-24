import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Search, Package, Ship as ShipIcon } from 'lucide-react';
import { cargoAPI, shipAPI } from '../../services/api';
import { Cargo, AddCargoRequest, UpdateCargoRequest, Ship } from '../../types';
import MainLayout from '../../components/Layout/MainLayout';
import CargoForm from './CargoForm';

const CargoList: React.FC = () => {
  const [cargos, setCargos] = useState<Cargo[]>([]);
  const [allCargos, setAllCargos] = useState<Cargo[]>([]);
  const [ships, setShips] = useState<Ship[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editingCargo, setEditingCargo] = useState<Cargo | null>(null);
  
  // Filtreler
  const [filterDescription, setFilterDescription] = useState('');
  const [filterCargoType, setFilterCargoType] = useState('');
  const [filterShipId, setFilterShipId] = useState('');

  const params = {
    description: filterDescription || undefined,
    cargoType: filterCargoType || undefined,
    shipId: filterShipId ? parseInt(filterShipId) : undefined,
    pageNumber: 1,
    pageSize: 100
  };

  useEffect(() => {
    loadAllShips();
  }, []);

  useEffect(() => {
    if (ships.length > 0) {
      loadCargos();
    }
  }, [filterDescription, filterCargoType, filterShipId, ships]);

  const loadAllShips = async () => {
    try {
      const shipsResponse = await shipAPI.getAll();
      const allShips = shipsResponse.data || [];
      console.log('🚢 All ships loaded:', allShips);
      setShips(allShips);
    } catch (error) {
      console.error('Error loading ships:', error);
      setShips([]);
    }
  };

  const loadCargos = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await cargoAPI.getAll(params);
      const response_all = await cargoAPI.getAll();
      
      const filteredCargos = response.data || [];
      const allCargosData = response_all.data || [];

      console.log('📦 Filtered Cargos loaded:', filteredCargos);
      console.log('📊 All Cargos loaded:', allCargosData);

      setAllCargos(allCargosData);
      setCargos(filteredCargos);

    } catch (error) {
      console.error('Error loading data:', error);
      setError('Veriler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };



  const handleCreate = async (data: AddCargoRequest) => {
    try {
      console.log('➕ Creating cargo with data:', data);
      await cargoAPI.create(data);
      console.log('✅ Cargo created successfully');
      setShowForm(false);
      loadCargos();
    } catch (error) {
      console.error('❌ Error creating cargo:', error);
      throw error;
    }
  };

  const handleUpdate = async (id: number, data: UpdateCargoRequest) => {
    try {
      console.log('✏️ Updating cargo with ID:', id, 'and data:', data);
      await cargoAPI.update(id, data);
      console.log('✅ Cargo updated successfully');
      setEditingCargo(null);
      loadCargos();
    } catch (error) {
      console.error('❌ Error updating cargo:', error);
      throw error;
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Bu yük kaydını silmek istediğinizden emin misiniz?')) {
      try {
        console.log('🗑️ Deleting cargo with ID:', id);
        await cargoAPI.delete(id);
        console.log('✅ Cargo deleted successfully');
        loadCargos();
      } catch (error) {
        console.error('❌ Error deleting cargo:', error);
      }
    }
  };

  // İstatistikler
  const totalCargos = allCargos.length;
  const totalWeight = allCargos.reduce((sum, cargo) => sum + cargo.weight, 0);
  const uniqueShips = [...new Set(allCargos.map(cargo => cargo.shipId))].length;
  const uniqueCargoTypes = [...new Set(allCargos.map(cargo => cargo.cargoType).filter(Boolean))];

  const CargoFilters = (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Açıklama Arama
        </label>
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="Yük açıklaması..."
            value={filterDescription}
            onChange={(e) => setFilterDescription(e.target.value)}
            className="input-field pl-10"
          />
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Yük Tipi
        </label>
        <select
          value={filterCargoType}
          onChange={(e) => setFilterCargoType(e.target.value)}
          className="input-field"
        >
          <option value="">Tüm Tipler</option>
          {uniqueCargoTypes.map(cargoType => (
            <option key={cargoType} value={cargoType}>{cargoType}</option>
          ))}
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Gemi
        </label>
        <select
          value={filterShipId}
          onChange={(e) => setFilterShipId(e.target.value)}
          className="input-field"
        >
          <option value="">Tüm Gemiler</option>
          {ships.map(ship => (
            <option key={ship.shipId} value={ship.shipId}>
              {ship.name} ({ship.imo})
            </option>
          ))}
        </select>
      </div>

      {/* Filtreleri Temizle Butonu */}
      <div className="pt-2">
        <button
          onClick={() => {
            setFilterDescription('');
            setFilterCargoType('');
            setFilterShipId('');
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
      <MainLayout sidebarContent={CargoFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Yük kayıtları yükleniyor...</p>
          </div>
        </div>
      </MainLayout>
    );
  }

  if (error) {
    return (
      <MainLayout sidebarContent={CargoFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="text-red-600 text-xl mb-4">⚠️</div>
            <p className="text-red-600 mb-4">{error}</p>
            <button 
              onClick={loadCargos}
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
    <MainLayout sidebarContent={CargoFilters}>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Yük Kayıtları</h1>
            <p className="text-gray-600">Gemi yüklerini yönetin</p>
          </div>
          <button
            onClick={() => setShowForm(true)}
            className="btn-primary flex items-center space-x-2"
          >
            <Plus className="h-4 w-4" />
            <span>Yeni Yük Ekle</span>
          </button>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Toplam Yük Kaydı</div>
            <div className="text-2xl font-bold text-gray-900">{totalCargos}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-blue-600">Toplam Ağırlık</div>
            <div className="text-2xl font-bold text-blue-600">{totalWeight.toFixed(2)} ton</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-green-600">Yüklü Gemi Sayısı</div>
            <div className="text-2xl font-bold text-green-600">{uniqueShips}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-purple-600">Yük Tipi Sayısı</div>
            <div className="text-2xl font-bold text-purple-600">{uniqueCargoTypes.length}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Filtrelenen</div>
            <div className="text-2xl font-bold text-gray-900">{cargos.length}</div>
          </div>
        </div>

        {/* Cargos Table */}
        <div className="card">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="table-header">Gemi</th>
                  <th className="table-header">Yük Tipi</th>
                  <th className="table-header">Ağırlık (ton)</th>
                  <th className="table-header">Açıklama</th>
                  <th className="table-header">İşlemler</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {cargos.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="table-cell text-center text-gray-500 py-8">
                      {filterDescription || filterCargoType || filterShipId
                        ? 'Arama kriterlerine uygun yük bulunamadı'
                        : 'Henüz yük kaydı eklenmemiş'}
                    </td>
                  </tr>
                ) : (
                  cargos.map((cargo) => (
                    <tr key={cargo.cargoId} className="hover:bg-gray-50">
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <ShipIcon className="h-4 w-4 text-blue-600" />
                          <div>
                            {(() => {
                              const ship = ships.find(s => s.shipId === cargo.shipId);
                              return (
                                <>
                                  <div className="font-medium">{ship?.name || 'Bilinmeyen Gemi'}</div>
                                  <div className="text-sm text-gray-500">{ship?.imo || 'IMO yok'}</div>
                                </>
                              );
                            })()}
                          </div>
                        </div>
                      </td>
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <Package className="h-4 w-4 text-purple-600" />
                          <span>{cargo.cargoType}</span>
                        </div>
                      </td>
                      <td className="table-cell">
                        <span className="font-medium">{cargo.weight.toFixed(2)}</span>
                      </td>
                      <td className="table-cell">
                        <div className="max-w-xs">
                          <span className="text-sm text-gray-900">
                            {cargo.description}
                          </span>
                        </div>
                      </td>
                      <td className="table-cell">
                        <div className="flex space-x-2">
                          <button
                            onClick={() => setEditingCargo(cargo)}
                            className="text-blue-600 hover:text-blue-800 p-1"
                            title="Düzenle"
                          >
                            <Edit className="h-4 w-4" />
                          </button>
                          <button
                            onClick={() => handleDelete(cargo.cargoId)}
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
      {(showForm || editingCargo) && (
        <CargoForm
          cargo={editingCargo}
          ships={ships}
          onSubmit={async (data) => {
            if (editingCargo) {
              await handleUpdate(editingCargo.cargoId, data);
            } else {
              await handleCreate(data);
            }
          }}
          onCancel={() => {
            setShowForm(false);
            setEditingCargo(null);
          }}
        />
      )}
    </MainLayout>
  );
};

export default CargoList;