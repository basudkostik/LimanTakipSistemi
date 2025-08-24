import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Search, User, Mail, Phone, Briefcase, Globe } from 'lucide-react';
import { crewMemberAPI } from '../../services/api';
import { CrewMember, AddCrewMemberRequest, UpdateCrewMemberRequest } from '../../types';
import MainLayout from '../../components/Layout/MainLayout';
import CrewMemberForm from './CrewMemberForm';

const CrewMemberList: React.FC = () => {
  const [crewMembers, setCrewMembers] = useState<CrewMember[]>([]);
  const [allCrewMembers, setAllCrewMembers] = useState<CrewMember[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editingCrewMember, setEditingCrewMember] = useState<CrewMember | null>(null);
  
  // Filtreler
  const [filterName, setFilterName] = useState('');
  const [filterPosition, setFilterPosition] = useState('');
  const [filterNationality, setFilterNationality] = useState('');

  const params = {
    name: filterName || undefined,
    position: filterPosition || undefined,
    nationality: filterNationality || undefined,
    pageNumber: 1,
    pageSize: 100
  };

  useEffect(() => {
    loadCrewMembers();
  }, []);

  useEffect(() => {
    loadCrewMembers();
  }, [filterName, filterPosition, filterNationality]);

  const loadCrewMembers = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await crewMemberAPI.getAll(params);
      const response_all = await crewMemberAPI.getAll();
      
      const filteredCrewMembers = response.data || [];
      const allCrewMembersData = response_all.data || [];

      console.log('👥 Filtered CrewMembers loaded:', filteredCrewMembers);
      console.log('📊 All CrewMembers loaded:', allCrewMembersData);

      setAllCrewMembers(allCrewMembersData);
      setCrewMembers(filteredCrewMembers);

    } catch (error) {
      console.error('Error loading data:', error);
      setError('Veriler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: AddCrewMemberRequest) => {
    try {
      console.log('➕ Creating crew member with data:', data);
      await crewMemberAPI.create(data);
      console.log('✅ Crew member created successfully');
      setShowForm(false);
      loadCrewMembers();
    } catch (error) {
      console.error('❌ Error creating crew member:', error);
      throw error;
    }
  };

  const handleUpdate = async (id: number, data: UpdateCrewMemberRequest) => {
    try {
      console.log('✏️ Updating crew member with ID:', id, 'and data:', data);
      await crewMemberAPI.update(id, data);
      console.log('✅ Crew member updated successfully');
      setEditingCrewMember(null);
      loadCrewMembers();
    } catch (error) {
      console.error('❌ Error updating crew member:', error);
      throw error;
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Bu mürettebat kaydını silmek istediğinizden emin misiniz?')) {
      try {
        console.log('🗑️ Deleting crew member with ID:', id);
        await crewMemberAPI.delete(id);
        console.log('✅ Crew member deleted successfully');
        loadCrewMembers();
      } catch (error) {
        console.error('❌ Error deleting crew member:', error);
      }
    }
  };

  // İstatistikler
  const totalCrewMembers = allCrewMembers.length;
  const uniquePositions = [...new Set(allCrewMembers.map(crew => crew.position).filter(Boolean))];
  const uniqueNationalities = [...new Set(allCrewMembers.map(crew => crew.nationality).filter(Boolean))];

  const CrewMemberFilters = (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          İsim Arama
        </label>
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="İsim veya soyisim..."
            value={filterName}
            onChange={(e) => setFilterName(e.target.value)}
            className="input-field pl-10"
          />
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Pozisyon
        </label>
        <select
          value={filterPosition}
          onChange={(e) => setFilterPosition(e.target.value)}
          className="input-field"
        >
          <option key="all-positions" value="">Tüm Pozisyonlar</option>
          {uniquePositions.map(position => (
            <option key={`position-${position}`} value={position}>{position}</option>
          ))}
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Uyrukluk
        </label>
        <select
          value={filterNationality}
          onChange={(e) => setFilterNationality(e.target.value)}
          className="input-field"
        >
          <option key="all-nationalities" value="">Tüm Uyruklar</option>
          {uniqueNationalities.map(nationality => (
            <option key={`nationality-${nationality}`} value={nationality}>{nationality}</option>
          ))}
        </select>
      </div>

      {/* Filtreleri Temizle Butonu */}
      <div className="pt-2">
        <button
          onClick={() => {
            setFilterName('');
            setFilterPosition('');
            setFilterNationality('');
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
      <MainLayout sidebarContent={CrewMemberFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Mürettebat kayıtları yükleniyor...</p>
          </div>
        </div>
      </MainLayout>
    );
  }

  if (error) {
    return (
      <MainLayout sidebarContent={CrewMemberFilters}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="text-red-600 text-xl mb-4">⚠️</div>
            <p className="text-red-600 mb-4">{error}</p>
            <button 
              onClick={loadCrewMembers}
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
    <MainLayout sidebarContent={CrewMemberFilters}>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Mürettebat Kayıtları</h1>
            <p className="text-gray-600">Mürettebat üyelerini yönetin</p>
          </div>
          <button
            onClick={() => setShowForm(true)}
            className="btn-primary flex items-center space-x-2"
          >
            <Plus className="h-4 w-4" />
            <span>Yeni Mürettebat Ekle</span>
          </button>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Toplam Mürettebat</div>
            <div className="text-2xl font-bold text-gray-900">{totalCrewMembers}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-blue-600">Farklı Pozisyon</div>
            <div className="text-2xl font-bold text-blue-600">{uniquePositions.length}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-green-600">Farklı Uyrukluk</div>
            <div className="text-2xl font-bold text-green-600">{uniqueNationalities.length}</div>
          </div>
          <div className="card">
            <div className="text-sm font-medium text-gray-500">Filtrelenen</div>
            <div className="text-2xl font-bold text-gray-900">{crewMembers.length}</div>
          </div>
        </div>

        {/* CrewMembers Table */}
        <div className="card">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="table-header">İsim</th>
                  <th className="table-header">Pozisyon</th>
                  <th className="table-header">Uyrukluk</th>
                  <th className="table-header">İletişim</th>
                  <th className="table-header">İşlemler</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {crewMembers.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="table-cell text-center text-gray-500 py-8">
                      {filterName || filterPosition || filterNationality
                        ? 'Arama kriterlerine uygun mürettebat bulunamadı'
                        : 'Henüz mürettebat kaydı eklenmemiş'}
                    </td>
                  </tr>
                ) : (
                  crewMembers.map((crewMember, index) => (
                    <tr key={crewMember.crewMemberId || `crewmember-${index}`} className="hover:bg-gray-50">
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <User className="h-4 w-4 text-blue-600" />
                          <div>
                            <div className="font-medium">{crewMember.name}</div>
                            <div className="text-sm text-gray-500">ID: {crewMember.crewMemberId}</div>
                          </div>
                        </div>
                      </td>
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <Briefcase className="h-4 w-4 text-purple-600" />
                          <span>{crewMember.position}</span>
                        </div>
                      </td>
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <Globe className="h-4 w-4 text-green-600" />
                          <span>{crewMember.nationality}</span>
                        </div>
                      </td>
                      <td className="table-cell">
                        <div className="space-y-1">
                          <div className="flex items-center space-x-1 text-sm">
                            <Mail className="h-3 w-3 text-gray-400" />
                            <span className="text-gray-600">{crewMember.email}</span>
                          </div>
                          <div className="flex items-center space-x-1 text-sm">
                            <Phone className="h-3 w-3 text-gray-400" />
                            <span className="text-gray-600">{crewMember.phone}</span>
                          </div>
                        </div>
                      </td>
                      <td className="table-cell">
                        <div className="flex space-x-2">
                          <button
                            onClick={() => setEditingCrewMember(crewMember)}
                            className="text-blue-600 hover:text-blue-800 p-1"
                            title="Düzenle"
                          >
                            <Edit className="h-4 w-4" />
                          </button>
                          <button
                            onClick={() => handleDelete(crewMember.crewMemberId)}
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
      {(showForm || editingCrewMember) && (
        <CrewMemberForm
          crewMember={editingCrewMember}
          onSubmit={async (data) => {
            if (editingCrewMember) {
              await handleUpdate(editingCrewMember.crewMemberId, data);
            } else {
              await handleCreate(data);
            }
          }}
          onCancel={() => {
            setShowForm(false);
            setEditingCrewMember(null);
          }}
        />
      )}
    </MainLayout>
  );
};

export default CrewMemberList; 