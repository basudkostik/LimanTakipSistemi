import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Search, User, Mail, Phone, Briefcase } from 'lucide-react';
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

  const [filterName, setFilterName] = useState('');
  const [filterRole, setFilterRole] = useState('');

  useEffect(() => {
    loadCrewMembers();
  }, []);

  useEffect(() => {
    filterCrewMembers();
  }, [filterName, filterRole, allCrewMembers]);

  const loadCrewMembers = async () => {
    try {
      setLoading(true);
      setError(null);

      const response_all = await crewMemberAPI.getAll(); 
      const allCrewMembersData = response_all.data || [];
      console.log('🚢 All members loaded:', allCrewMembersData);
      setAllCrewMembers(allCrewMembersData);
      setLoading(false);
    } catch (error) {
      console.error('Error loading data:', error);
      setError('Veriler yüklenirken hata oluştu');
      setLoading(false);
    }
  };

  const filterCrewMembers = () => {
    const search = filterName.toLowerCase();

    let filtered = allCrewMembers;

    if (search) {
      filtered = filtered.filter(member =>
        member.firstName.toLowerCase().includes(search) ||
        member.lastName.toLowerCase().includes(search)
      );
    }

    if (filterRole) {
      filtered = filtered.filter(member => member.role === filterRole);
    }

    setCrewMembers(filtered);
  };

  const handleCreate = async (data: AddCrewMemberRequest) => {
    try {
      await crewMemberAPI.create(data);
      setShowForm(false);
      await loadCrewMembers();
    } catch (error) {
      console.error('Error creating crew member:', error);
      throw error;
    }
  };

  const handleUpdate = async (id: number, data: UpdateCrewMemberRequest) => {
    try {
      await crewMemberAPI.update(id, data);
      setEditingCrewMember(null);
      await loadCrewMembers();
    } catch (error) {
      console.error('Error updating crew member:', error);
      throw error;
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Bu mürettebat kaydını silmek istediğinizden emin misiniz?')) {
      try {
        await crewMemberAPI.delete(id);
        await loadCrewMembers();
      } catch (error) {
        console.error('Error deleting crew member:', error);
      }
    }
  };

  const totalCrewMembers = allCrewMembers.length;
  const uniquePositions = [...new Set(allCrewMembers.map(crew => crew.role).filter(Boolean))];

  const CrewMemberFilters = (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          İsim veya Soyisim Arama
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
          value={filterRole}
          onChange={(e) => setFilterRole(e.target.value)}
          className="input-field"
        >
          <option key="all-positions" value="">Tüm Pozisyonlar</option>
          {uniquePositions.map(position => (
            <option key={`position-${position}`} value={position}>{position}</option>
          ))}
        </select>
      </div>

      <div className="pt-2">
        <button
          onClick={() => {
            setFilterName('');
            setFilterRole('');
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
            <div className="text-sm font-medium text-gray-500">Filtrelenen</div>
            <div className="text-2xl font-bold text-gray-900">{crewMembers.length}</div>
          </div>
        </div>

        <div className="card">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="table-header">İsim</th>
                  <th className="table-header">Pozisyon</th>
                  <th className="table-header">İletişim</th>
                  <th className="table-header">İşlemler</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {crewMembers.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="table-cell text-center text-gray-500 py-8">
                      {(filterName || filterRole)
                        ? 'Arama kriterlerine uygun mürettebat bulunamadı'
                        : 'Henüz mürettebat kaydı eklenmemiş'}
                    </td>
                  </tr>
                ) : (
                  crewMembers.map((crewMember, index) => (
                    <tr key={crewMember.crewId || `crewmember-${index}`} className="hover:bg-gray-50">
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <User className="h-4 w-4 text-blue-600" />
                          <div>
                            <div className="font-medium">{crewMember.firstName + " " + crewMember.lastName}</div>
                            <div className="text-sm text-gray-500">ID: {crewMember.crewId}</div>
                          </div>
                        </div>
                      </td>
                      <td className="table-cell">
                        <div className="flex items-center space-x-2">
                          <Briefcase className="h-4 w-4 text-purple-600" />
                          <span>{crewMember.role}</span>
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
                            <span className="text-gray-600">{crewMember.phoneNumber}</span>
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
                            onClick={() => handleDelete(crewMember.crewId)}
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

      {(showForm || editingCrewMember) && (
        <CrewMemberForm
          crewMember={editingCrewMember}
          onSubmit={async (data) => {
            if (editingCrewMember) {
              await handleUpdate(editingCrewMember.crewId, data);
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
