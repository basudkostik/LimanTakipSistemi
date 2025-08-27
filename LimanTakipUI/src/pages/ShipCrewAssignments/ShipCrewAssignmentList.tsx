import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Search, Calendar, Ship as ShipIcon, User } from 'lucide-react';
import { shipCrewAssignmentAPI, shipAPI, crewMemberAPI } from '../../services/api';
import { ShipCrewAssignment, AddShipCrewAssignmentRequest, UpdateShipCrewAssignmentRequest, Ship, CrewMember } from '../../types';
import MainLayout from '../../components/Layout/MainLayout';
import ShipCrewAssignmentForm from './ShipCrewAssignmentForm';

const ShipCrewAssignmentList: React.FC = () => {
  const [assignments, setAssignments] = useState<ShipCrewAssignment[]>([]);
  const [allAssignments, setAllAssignments] = useState<ShipCrewAssignment[]>([]);
  const [ships, setShips] = useState<Ship[]>([]);
  const [crewMembers, setCrewMembers] = useState<CrewMember[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editingAssignment, setEditingAssignment] = useState<ShipCrewAssignment | null>(null);
  
  // Filtreler
  const [filterShipId, setFilterShipId] = useState('');
  const [filterCrewId, setFilterCrewId] = useState('');
  const [filterAssignmentDate, setFilterAssignmentDate] = useState('');

  const params = {
    shipId: filterShipId ? parseInt(filterShipId) : undefined,
    crewId: filterCrewId ? parseInt(filterCrewId) : undefined,
    assignmentDate: filterAssignmentDate || undefined,
    pageNumber: 1,
    pageSize: 100
  };

  useEffect(() => {
    loadShipsAndCrewMembers();
  }, []);

  useEffect(() => {
    if (ships.length > 0 && crewMembers.length > 0) {
      loadAssignments();
    }
  }, [filterShipId, filterCrewId, filterAssignmentDate, ships, crewMembers]);

  const loadShipsAndCrewMembers = async () => {
    try {
      const [shipsResponse, crewMembersResponse] = await Promise.all([
        shipAPI.getAll(),
        crewMemberAPI.getAll()
      ]);

      const allShips = shipsResponse.data || [];
      const allCrewMembers = crewMembersResponse.data || [];

      console.log('🚢 Ships loaded:', allShips);
      console.log('👥 CrewMembers loaded:', allCrewMembers);

      setShips(allShips);
      setCrewMembers(allCrewMembers);
      
      if (allShips.length > 0 && allCrewMembers.length > 0) {
        loadAssignments();
      }
    } catch (error) {
      console.error('Error loading ships and crew members:', error);
    }
  };

  const loadAssignments = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await shipCrewAssignmentAPI.getAll(params);
      const response_all = await shipCrewAssignmentAPI.getAll();
      
      const filteredAssignments = response.data || [];
      const allAssignmentsData = response_all.data || [];

      console.log('📋 Filtered Assignments loaded:', filteredAssignments);
      console.log('📊 All Assignments loaded:', allAssignmentsData);

      console.log('📦 API çağrısına gönderilen filtre parametreleri:', params);

      const assignmentsWithRelations = filteredAssignments.map(assignment => ({
        ...assignment,
        ship: ships.find(ship => ship.shipId === assignment.shipId),
        crewMember: crewMembers.find(crew => crew.crewId === assignment.crewId)
      }));

      const allAssignmentsWithRelations = allAssignmentsData.map(assignment => ({
        ...assignment,
        ship: ships.find(ship => ship.shipId === assignment.shipId),
        crewMember: crewMembers.find(crew => crew.crewId === assignment.crewId)
      }));

      setAllAssignments(allAssignmentsWithRelations);
      setAssignments(assignmentsWithRelations);

    } catch (error) {
      console.error('Error loading data:', error);
      setError('Veriler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: AddShipCrewAssignmentRequest) => {
    try {
      console.log('➕ Creating assignment with data:', data);
      await shipCrewAssignmentAPI.create(data);
      console.log('✅ Assignment created successfully');
      setShowForm(false);
      loadAssignments();
    } catch (error) {
      console.error('❌ Error creating assignment:', error);
      throw error;
    }
  };

  const handleUpdate = async (id: number, data: UpdateShipCrewAssignmentRequest) => {
    try {
      console.log('✏️ Updating assignment with ID:', id, 'and data:', data);
      await shipCrewAssignmentAPI.update(id, data);
      console.log('✅ Assignment updated successfully');
      setEditingAssignment(null);
      loadAssignments();
    } catch (error) {
      console.error('❌ Error updating assignment:', error);
      throw error;
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Bu atama kaydını silmek istediğinizden emin misiniz?')) {
      try {
        console.log('🗑️ Deleting assignment with ID:', id);
        await shipCrewAssignmentAPI.delete(id);
        console.log('✅ Assignment deleted successfully');
        loadAssignments();
      } catch (error) {
        console.error('❌ Error deleting assignment:', error);
      }
    }
  };

  
  const totalAssignments = allAssignments.length;
  const uniqueCrewMembers = [...new Set(allAssignments.map(assignment => assignment.crewId))].length;
  const activeShips = ships.filter(ship => 
    allAssignments.some(assignment => assignment.shipId === ship.shipId)
  ).length;

  const AssignmentFilters = (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Gemi
        </label>
        <select
          value={filterShipId}
          onChange={(e) => setFilterShipId(e.target.value)}
          className="input-field"
        >
          <option key="all-ships" value="">Tüm Gemiler</option>
          {ships.map((ship, index) => (
            <option key={`ship-${ship.shipId || `temp-${index}`}`} value={ship.shipId}>
              {ship.name} ({ship.imo})
            </option>
          ))}
        </select>

        {/* Seçilen gemi ID'sini göster */}
  <p className="mt-2 text-gray-600">
    Seçilen Gemi ID: {filterShipId || 'Yok'}
  </p>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Mürettebat
        </label>
        <select
          value={filterCrewId}
          onChange={(e) => setFilterCrewId(e.target.value)}
          className="input-field"
        >
          <option key="all-crew" value="">Tüm Mürettebat</option>
          {crewMembers.map((crewMember, index) => (
            <option key={`crew-${crewMember.crewId || `temp-${index}`}`} value={crewMember.crewId}>
              {crewMember.firstName +" "+crewMember.lastName} - {crewMember.role}
            </option>
          ))}
        </select>
        {/* Seçilen crew ID'sini göster */}
  <p className="mt-2 text-gray-600">
    Seçilen Crew ID: {filterCrewId || 'Yok'}
  </p>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Atama Tarihi
        </label>
        <input
          type="date"
          value={filterAssignmentDate}
          onChange={(e) => setFilterAssignmentDate(e.target.value)}
          className="input-field"
        />
      </div>

      {/* Filtreleri Temizle Butonu */}
      <div className="pt-2">
        <button
          onClick={() => {
            setFilterShipId('');
            setFilterCrewId('');
            setFilterAssignmentDate('');
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
      <MainLayout sidebarContent={AssignmentFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Atama kayıtları yükleniyor...</p>
          </div>
        </div>
      </MainLayout>
    );
  }

  if (error) {
    return (
      <MainLayout sidebarContent={AssignmentFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="text-red-600 text-xl mb-4">⚠️</div>
            <p className="text-red-600 mb-4">{error}</p>
            <button 
              onClick={loadAssignments}
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
    <MainLayout sidebarContent={AssignmentFilters}>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Gemi-Mürettebat Atamaları</h1>
            <p className="text-gray-600">Mürettebat atamalarını yönetin</p>
          </div>
          <button
            onClick={() => setShowForm(true)}
            className="btn-primary flex items-center space-x-2"
          >
            <Plus className="h-4 w-4" />
            <span>Yeni Atama Ekle</span>
          </button>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Toplam Atama</div>
            <div className="text-2xl font-bold text-gray-900">{totalAssignments}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-blue-600">Aktif Gemi</div>
            <div className="text-2xl font-bold text-blue-600">{activeShips}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-green-600">Görevli Mürettebat</div>
            <div className="text-2xl font-bold text-green-600">{uniqueCrewMembers}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Filtrelenen</div>
            <div className="text-2xl font-bold text-gray-900">{assignments.length}</div>
          </div>
        </div>

        {/* Assignments Table */}
        <div className="card">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="table-header">Gemi</th>
                  <th className="table-header">Mürettebat</th>
                  <th className="table-header">Pozisyon</th>
                  <th className="table-header">Atama Tarihi</th>
                  <th className="table-header">İşlemler</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {assignments.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="table-cell text-center text-gray-500 py-8">
                      {filterShipId || filterCrewId || filterAssignmentDate
                        ? 'Arama kriterlerine uygun atama bulunamadı'
                        : 'Henüz atama kaydı eklenmemiş'}
                    </td>
                  </tr>
                ) : (
                  assignments.map((assignment, index) => (  
                    <tr key={assignment.assignmentId || `assignment-${index}`} className="hover:bg-gray-50">
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <ShipIcon className="h-4 w-4 text-blue-600" />
                          <div>
                            <div className="font-medium">{assignment.ship?.name || 'Bilinmeyen Gemi'}</div>
                            <div className="text-sm text-gray-500">{assignment.ship?.imo || 'IMO yok'}</div>
                          </div>
                        </div>
                      </td>
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <User className="h-4 w-4 text-green-600" />
                          <div>
                            <div className="font-medium">{ (assignment.crewMember?.firstName+" "+assignment.crewMember?.lastName  )  || 'Bilinmeyen Mürettebat'}</div>
                          </div>
                        </div>
                      </td>
                      <td className="table-cell">
                        <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-purple-100 text-purple-800">
                          {assignment.crewMember?.role || 'Bilinmeyen'}
                        </span>
                      </td>
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <Calendar className="h-4 w-4 text-gray-400" />
                          <span>{new Date(assignment.assignmentDate).toLocaleDateString('tr-TR')}</span>
                        </div>
                      </td>
                      <td className="table-cell">
                        <div className="flex space-x-2">
                          <button
                            onClick={() => setEditingAssignment(assignment)}
                            className="text-blue-600 hover:text-blue-800 p-1"
                            title="Düzenle"
                          >
                            <Edit className="h-4 w-4" />
                          </button>
                          <button
                            onClick={() => handleDelete(assignment.assignmentId)}
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
      {(showForm || editingAssignment) && (
        <ShipCrewAssignmentForm
          assignment={editingAssignment}
          ships={ships}
          crewMembers={crewMembers}
          existingAssignments={allAssignments}
          onSubmit={async (data) => {
            if (editingAssignment) {
              await handleUpdate(editingAssignment.assignmentId, data);
            } else {
              await handleCreate(data);
            }
          }}
          onCancel={() => {
            setShowForm(false);
            setEditingAssignment(null);
          }}
        />
      )}
    </MainLayout>
  );
};

export default ShipCrewAssignmentList;