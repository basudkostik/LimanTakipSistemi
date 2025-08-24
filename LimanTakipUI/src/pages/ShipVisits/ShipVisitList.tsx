import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Search, Calendar, Ship as ShipIcon, Anchor } from 'lucide-react';
import { shipVisitAPI, shipAPI, portAPI } from '../../services/api';
import { ShipVisit, AddShipVisitRequest, UpdateShipVisitRequest, Ship, Port } from '../../types';
import MainLayout from '../../components/Layout/MainLayout';
import ShipVisitForm from './ShipVisitForm';

const ShipVisitList: React.FC = () => {
  const [shipVisits, setShipVisits] = useState<ShipVisit[]>([]);
  const [allShipVisits, setAllShipVisits] = useState<ShipVisit[]>([]);
  const [ships, setShips] = useState<Ship[]>([]);
  const [ports, setPorts] = useState<Port[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editingShipVisit, setEditingShipVisit] = useState<ShipVisit | null>(null);
  const [currentTime, setCurrentTime] = useState(new Date());
  
  // Filtreler
  const [filterShip, setFilterShip] = useState('');
  const [filterPort, setFilterPort] = useState('');
  const [filterArrivalDate, setFilterArrivalDate] = useState('');
  const [filterDepartureDate, setFilterDepartureDate] = useState('');

  
  useEffect(() => {
    loadShipsAndPorts();
  }, []);

  // Sistem saatini her saniye güncelle
  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);

    return () => clearInterval(timer);
  }, []);

  
  useEffect(() => {
    if (ships.length > 0 && ports.length > 0) {
      loadShipVisits();
    }
  }, [filterShip, filterPort, filterArrivalDate, filterDepartureDate, ships, ports]);

  const loadShipsAndPorts = async () => {
    try {
      const [shipsResponse, portsResponse] = await Promise.all([
        shipAPI.getAll(),
        portAPI.getAll()
      ]);

      const allShips = shipsResponse.data || [];
      const allPorts = portsResponse.data || [];

      console.log('🚢 Ships loaded:', allShips);
      console.log('⚓ Ports loaded:', allPorts);

      setShips(allShips);
      setPorts(allPorts);
      
      if (allShips.length > 0 && allPorts.length > 0) {
        loadShipVisits();
      }
    } catch (error) {
      console.error('Error loading ships and ports:', error);
    }
  };

  const loadShipVisits = async () => {
    try {
      setLoading(true);
      setError(null);

      const params: any = {
        pageNumber: 1,
        pageSize: 100
      };

      if (filterShip) {
        const matchingShip = ships.find(ship => 
          ship.name?.toLowerCase().includes(filterShip.toLowerCase()) ||
          ship.imo?.toLowerCase().includes(filterShip.toLowerCase())
        );
        if (matchingShip) {
          params.shipId = matchingShip.shipId;
        }
      }

      if (filterPort) {
        const matchingPort = ports.find(port => 
          port.name?.toLowerCase().includes(filterPort.toLowerCase()) ||
          port.city?.toLowerCase().includes(filterPort.toLowerCase()) ||
          port.country?.toLowerCase().includes(filterPort.toLowerCase())
        );
        if (matchingPort) {
          params.portId = matchingPort.portId;
        }
      }

      if (filterArrivalDate) {
        params.arrivalDate = filterArrivalDate;
      }
      if (filterDepartureDate) {
        params.departureDate = filterDepartureDate;
      }

      console.log('🔍 Sending params to API:', params);

      const shipVisitsResponse = await shipVisitAPI.getAll(params);
      const allShipVisitsResponse = await shipVisitAPI.getAll();

      const filteredVisits = shipVisitsResponse.data || [];
      const allVisits = allShipVisitsResponse.data || [];

      console.log('🔍 API Response - Filtered visits:', shipVisitsResponse);
      console.log('🔍 API Response - All visits:', allShipVisitsResponse);
      console.log('🔍 Debug - Ships state:', ships);
      console.log('🔍 Debug - Ports state:', ports);
      console.log('🔍 Debug - Filtered visits:', filteredVisits);

      const visitsWithRelations = filteredVisits.map(visit => {
        const foundShip = ships.find(ship => ship.shipId === visit.shipId);
        const foundPort = ports.find(port => port.portId === visit.portId);
        
        console.log(`🔍 Visit ${visit.shipVisitId}: shipId=${visit.shipId}, foundShip=`, foundShip);
        console.log(`🔍 Visit ${visit.shipVisitId}: portId=${visit.portId}, foundPort=`, foundPort);
        
        return {
          ...visit,
          ship: foundShip,
          port: foundPort
        };
      });

      const allVisitsWithRelations = allVisits.map(visit => ({
        ...visit,
        ship: ships.find(ship => ship.shipId === visit.shipId),
        port: ports.find(port => port.portId === visit.portId)
      }));

      console.log('🚢 Filtered Ship Visits loaded:', filteredVisits);
      console.log('📊 All Ship Visits loaded:', allVisits);
      console.log('🔗 Visits with relations:', visitsWithRelations);

      setAllShipVisits(allVisitsWithRelations);
      setShipVisits(visitsWithRelations);
    } catch (error) {
      console.error('Error loading data:', error);
      setError('Veriler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: AddShipVisitRequest) => {
    try {
      console.log('➕ Creating ship visit with data:', data);
      await shipVisitAPI.create(data);
      console.log('✅ Ship visit created successfully');
      setShowForm(false);
      loadShipVisits();
    } catch (error) {
      console.error('❌ Error creating ship visit:', error);
      throw error;
    }
  };

  const handleUpdate = async (id: number, data: UpdateShipVisitRequest) => {
    try {
      console.log('✏️ Updating ship visit with ID:', id, 'and data:', data);
      await shipVisitAPI.update(id, data);
      console.log('✅ Ship visit updated successfully');
      setEditingShipVisit(null);
      loadShipVisits();
    } catch (error) {
      console.error('❌ Error updating ship visit:', error);
      throw error;
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Bu ziyaret kaydını silmek istediğinizden emin misiniz?')) {
      try {
        console.log('🗑️ Deleting ship visit with ID:', id);
        await shipVisitAPI.delete(id);
        console.log('✅ Ship visit deleted successfully');
        loadShipVisits();
      } catch (error) {
        console.error('❌ Error deleting ship visit:', error);
      }
    }
  };

  
  // Gemi durumunu kontrol eden fonksiyon
  const getShipVisitStatus = (visit: ShipVisit) => {
    const arrivalDate = new Date(visit.arrivalDate);
    const departureDate = new Date(visit.departureDate);
    
    console.log(`🔍 Durum kontrolü - Gemi: ${visit.ship?.name || 'Bilinmeyen'}`);
    console.log(`🔍 Sistem saati: ${currentTime.toLocaleString('tr-TR')}`);
    console.log(`🔍 Varış tarihi: ${arrivalDate.toLocaleString('tr-TR')}`);
    console.log(`🔍 Ayrılış tarihi: ${departureDate.toLocaleString('tr-TR')}`);
    
    // Henüz varış yapmamış
    if (currentTime < arrivalDate) {
      console.log(`🔍 Durum: Planlanmış`);
      return { status: 'Planlanmış', color: 'text-blue-600', bgColor: 'bg-blue-100' };
    }
    
    // Varış yapmış ama henüz ayrılmamış (limanda)
    if (currentTime >= arrivalDate && currentTime < departureDate) {
      console.log(`🔍 Durum: Limanda`);
      return { status: 'Limanda', color: 'text-green-600', bgColor: 'bg-green-100' };
    }
    
    // Ayrılış yapmış
    if (currentTime >= departureDate) {
      console.log(`🔍 Durum: Tamamlandı`);
      return { status: 'Tamamlandı', color: 'text-gray-600', bgColor: 'bg-gray-100' };
    }
    
    return { status: 'Bilinmiyor', color: 'text-gray-600', bgColor: 'bg-gray-100' };
  };

  const totalVisits = allShipVisits.length;
  const activeVisits = allShipVisits.filter(visit => {
    const status = getShipVisitStatus(visit);
    return status.status === 'Limanda';
  }).length;
  const plannedVisits = allShipVisits.filter(visit => {
    const status = getShipVisitStatus(visit);
    return status.status === 'Planlanmış';
  }).length;
  const completedVisits = allShipVisits.filter(visit => {
    const status = getShipVisitStatus(visit);
    return status.status === 'Tamamlandı';
  }).length;
  const uniqueShips = [...new Set(allShipVisits.map(visit => visit.shipId))].length;
  const uniquePorts = [...new Set(allShipVisits.map(visit => visit.portId))].length;

  const ShipVisitFilters = (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Gemi Arama
        </label>
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="Gemi adı veya IMO..."
            value={filterShip}
            onChange={(e) => setFilterShip(e.target.value)}
            className="input-field pl-10"
          />
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Liman Arama
        </label>
        <div className="relative">
          <Anchor className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="Liman adı, şehir veya ülke..."
            value={filterPort}
            onChange={(e) => setFilterPort(e.target.value)}
            className="input-field pl-10"
          />
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Varış Tarihi
        </label>
        <input
          type="date"
          value={filterArrivalDate}
          onChange={(e) => setFilterArrivalDate(e.target.value)}
          className="input-field"
        />
      </div>

             <div>
         <label className="block text-sm font-medium text-gray-700 mb-2">
           Ayrılış Tarihi
         </label>
         <input
           type="date"
           value={filterDepartureDate}
           onChange={(e) => setFilterDepartureDate(e.target.value)}
           className="input-field"
         />
       </div>

               {/* Filtreleri Temizle Butonu */}
        <div className="pt-2">
          <button
            onClick={() => {
              setFilterShip('');
              setFilterPort('');
              setFilterArrivalDate('');
              setFilterDepartureDate('');
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
      <MainLayout sidebarContent={ShipVisitFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Ziyaret kayıtları yükleniyor...</p>
          </div>
        </div>
      </MainLayout>
    );
  }

  if (error) {
    return (
      <MainLayout sidebarContent={ShipVisitFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="text-red-600 text-xl mb-4">⚠️</div>
            <p className="text-red-600 mb-4">{error}</p>
                         <button 
               onClick={loadShipVisits}
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
    <MainLayout sidebarContent={ShipVisitFilters}>
      <div className="space-y-6">
                 {/* Header */}
         <div className="flex justify-between items-center">
           <div>
             <h1 className="text-2xl font-bold text-gray-900">Ziyaret Kayıtları</h1>
             <p className="text-gray-600">Gemi liman ziyaretlerini yönetin</p>
             <p className="text-sm text-gray-500 mt-1">
               Sistem Saati: {currentTime.toLocaleString('tr-TR')}
             </p>
           </div>
           <button
             onClick={() => setShowForm(true)}
             className="btn-primary flex items-center space-x-2"
           >
             <Plus className="h-4 w-4" />
             <span>Yeni Ziyaret Ekle</span>
           </button>
         </div>

                 {/* Stats */}
         <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
           <div className="card">
             <div className="text-sm font-medium text-gray-500">Toplam Ziyaret</div>
             <div className="text-2xl font-bold text-gray-900">{totalVisits}</div>
           </div>
           <div className="card">
             <div className="text-sm font-medium text-blue-600">Planlanmış</div>
             <div className="text-2xl font-bold text-blue-600">{plannedVisits}</div>
           </div>
           <div className="card">
             <div className="text-sm font-medium text-green-600">Limanda</div>
             <div className="text-2xl font-bold text-green-600">{activeVisits}</div>
           </div>
           <div className="card">
             <div className="text-sm font-medium text-gray-600">Tamamlanan</div>
             <div className="text-2xl font-bold text-gray-600">{completedVisits}</div>
           </div>
           <div className="card">
             <div className="text-sm font-medium text-gray-500">Farklı Gemi</div>
             <div className="text-2xl font-bold text-gray-900">{uniqueShips}</div>
           </div>
         </div>

        {/* Ship Visits Table */}
        <div className="card">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
                             <thead className="bg-gray-50">
                 <tr>
                   <th className="table-header">Gemi</th>
                   <th className="table-header">Liman</th>
                   <th className="table-header">Varış Tarihi</th>
                   <th className="table-header">Ayrılış Tarihi</th>
                   <th className="table-header">Ziyaret Amacı</th>
                   <th className="table-header">Durum</th>
                   <th className="table-header">İşlemler</th>
                 </tr>
               </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                                 {shipVisits.length === 0 ? (
                   <tr>
                                          <td colSpan={7} className="table-cell text-center text-gray-500 py-8">
                        {filterShip || filterPort || filterArrivalDate || filterDepartureDate 
                          ? 'Arama kriterlerine uygun ziyaret bulunamadı' 
                          : 'Henüz ziyaret kaydı eklenmemiş'}
                      </td>
                   </tr>
                ) : (
                                       shipVisits.map((visit, index) => {
                      const visitStatus = getShipVisitStatus(visit);

                     return (
                                               <tr key={`visit-${visit.visitId}-${index}`} className="hover:bg-gray-50">
                                                 <td className="table-cell">
                           <div className="flex items-center space-x-2">
                             <ShipIcon className="h-4 w-4 text-blue-600" />
                             <div>
                               <div className="font-medium">{visit.ship?.name || 'Bilinmeyen Gemi'}</div>
                               <div className="text-sm text-gray-500">{visit.ship?.imo || 'IMO yok'}</div>
                             </div>
                           </div>
                         </td>
                        <td className="table-cell">
                          <div className="flex items-center space-x-2">
                            <Anchor className="h-4 w-4 text-green-600" />
                            <div>
                              <div className="font-medium">{visit.port?.name || 'Bilinmeyen Liman'}</div>
                              <div className="text-sm text-gray-500">
                                {visit.port?.city}, {visit.port?.country}
                              </div>
                            </div>
                          </div>
                        </td>
                        <td className="table-cell">
                          <div className="flex items-center space-x-2">
                            <Calendar className="h-4 w-4 text-gray-400" />
                            <span>{new Date(visit.arrivalDate).toLocaleDateString('tr-TR')}</span>
                          </div>
                        </td>
                                                 <td className="table-cell">
                           <div className="flex items-center space-x-2">
                             <Calendar className="h-4 w-4 text-gray-400" />
                             <span>{new Date(visit.departureDate).toLocaleDateString('tr-TR')}</span>
                           </div>
                         </td>
                         <td className="table-cell">
                           <div className="max-w-xs">
                             <span className="text-sm text-gray-900">
                               {visit.purpose || 'Amaç belirtilmemiş'}
                             </span>
                           </div>
                         </td>
                         <td className="table-cell">
                           <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${visitStatus.color} ${visitStatus.bgColor}`}>
                             {visitStatus.status}
                           </span>
                         </td>
                        <td className="table-cell">
                          <div className="flex space-x-2">
                            <button
                              onClick={() => {
                                console.log('🔍 Edit button clicked for visit:', visit);
                                setEditingShipVisit(visit);
                              }}
                              className="text-blue-600 hover:text-blue-800 p-1"
                              title="Düzenle"
                            >
                              <Edit className="h-4 w-4" />
                            </button>
                                                         <button
                               onClick={() => handleDelete(visit.visitId)}
                               className="text-red-600 hover:text-red-800 p-1"
                               title="Sil"
                             >
                              <Trash2 className="h-4 w-4" />
                            </button>
                          </div>
                        </td>
                      </tr>
                    );
                  })
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* Create/Edit Form Modal */}
      {(showForm || editingShipVisit) && (
        <ShipVisitForm
          shipVisit={editingShipVisit}
          ships={ships}
          ports={ports}
                     onSubmit={async (data) => {
             console.log('🔍 onSubmit called with editingShipVisit:', editingShipVisit);
             if (editingShipVisit) {
               console.log('🔍 Updating ship visit with ID:', editingShipVisit.visitId);
               await handleUpdate(editingShipVisit.visitId, data);
             } else {
               console.log('🔍 Creating new ship visit');
               await handleCreate(data);
             }
           }}
          onCancel={() => {
            setShowForm(false);
            setEditingShipVisit(null);
          }}
        />
      )}
    </MainLayout>
  );
};

export default ShipVisitList; 