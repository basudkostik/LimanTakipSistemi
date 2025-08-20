import React, { useState, useEffect } from 'react';
import { Ship, Anchor, Users, Package, Calendar, Settings, TrendingUp, Activity } from 'lucide-react';
import { Link } from 'react-router-dom';
import MainLayout from '../../components/Layout/MainLayout';
import { shipAPI, portAPI, shipVisitAPI, cargoAPI, crewMemberAPI, shipCrewAssignmentAPI } from '../../services/api';

interface DashboardStats {
  ships: number;
  ports: number;
  visits: number;
  cargo: number;
  crew: number;
  assignments: number;
}

const Dashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats>({
    ships: 0,
    ports: 0,
    visits: 0,
    cargo: 0,
    crew: 0,
    assignments: 0,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadDashboardStats();
  }, []);

  const loadDashboardStats = async () => {
    try {
      setLoading(true);
      setError(null);
      console.log('🔄 Dashboard stats yükleniyor...');
      
      const [
        shipsResponse,
        portsResponse,
        visitsResponse,
        cargoResponse,
        crewResponse,
        assignmentsResponse
      ] = await Promise.all([
        shipAPI.getAll(),
        portAPI.getAll(),
        shipVisitAPI.getAll(),
        cargoAPI.getAll(),
        crewMemberAPI.getAll(),
        shipCrewAssignmentAPI.getAll(),
      ]);

      console.log('📊 API Yanıtları:', {
        ships: shipsResponse.data,
        ports: portsResponse.data,
        visits: visitsResponse.data,
        cargo: cargoResponse.data,
        crew: crewResponse.data,
        assignments: assignmentsResponse.data,
      });

      // Safe data extraction with null checks
      const shipsData = shipsResponse.data || [];
      const portsData = portsResponse.data || [];
      const visitsData = visitsResponse.data || [];
      const cargoData = cargoResponse.data || [];
      const crewData = crewResponse.data || [];
      const assignmentsData = assignmentsResponse.data || [];

      setStats({
        ships: Array.isArray(shipsData) ? shipsData.length : 0,
        ports: Array.isArray(portsData) ? portsData.length : 0,
        visits: Array.isArray(visitsData) ? visitsData.length : 0,
        cargo: Array.isArray(cargoData) ? cargoData.length : 0,
        crew: Array.isArray(crewData) ? crewData.length : 0,
        assignments: Array.isArray(assignmentsData) ? assignmentsData.length : 0,
      });

      console.log('✅ Stats güncellendi:', {
        ships: Array.isArray(shipsData) ? shipsData.length : 0,
        ports: Array.isArray(portsData) ? portsData.length : 0,
        visits: Array.isArray(visitsData) ? visitsData.length : 0,
        cargo: Array.isArray(cargoData) ? cargoData.length : 0,
        crew: Array.isArray(crewData) ? crewData.length : 0,
        assignments: Array.isArray(assignmentsData) ? assignmentsData.length : 0,
      });

    } catch (error) {
      console.error('❌ Dashboard stats yüklenirken hata:', error);
      setError('Dashboard istatistikleri yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const quickActions = [
    {
      title: 'Yeni Gemi Ekle',
      description: 'Sisteme yeni gemi kaydı ekleyin',
      icon: Ship,
      href: '/ships',
      color: 'bg-blue-500',
      textColor: 'text-blue-500',
    },
    {
      title: 'Liman Ekle',
      description: 'Yeni liman bilgisi ekleyin',
      icon: Anchor,
      href: '/ports',
      color: 'bg-green-500',
      textColor: 'text-green-500',
    },
    {
      title: 'Ziyaret Kaydı',
      description: 'Gemi ziyaret bilgisi ekleyin',
      icon: Calendar,
      href: '/visits',
      color: 'bg-purple-500',
      textColor: 'text-purple-500',
    },
    {
      title: 'Yük Ekle',
      description: 'Gemi yük bilgisi ekleyin',
      icon: Package,
      href: '/cargo',
      color: 'bg-orange-500',
      textColor: 'text-orange-500',
    },
    {
      title: 'Mürettebat Ekle',
      description: 'Yeni mürettebat kaydı ekleyin',
      icon: Users,
      href: '/crew',
      color: 'bg-indigo-500',
      textColor: 'text-indigo-500',
    },
    {
      title: 'Atama Yap',
      description: 'Gemi-mürettebat ataması yapın',
      icon: Settings,
      href: '/assignments',
      color: 'bg-pink-500',
      textColor: 'text-pink-500',
    },
  ];

  if (loading) {
    return (
      <MainLayout showSidebar={false}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Dashboard yükleniyor...</p>
          </div>
        </div>
      </MainLayout>
    );
  }

  if (error) {
    return (
      <MainLayout showSidebar={false}>
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="text-red-600 text-xl mb-4">⚠️</div>
            <p className="text-red-600 mb-4">{error}</p>
            <button 
              onClick={loadDashboardStats}
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
    <MainLayout showSidebar={false}>
      <div className="space-y-6">
        {/* Header */}
        <div className="text-center">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">Dashboard</h1>
          <p className="text-gray-600">Liman Takip Sistemi Genel Bakış</p>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div className="card">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-blue-100">
                <Ship className="h-6 w-6 text-blue-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Toplam Gemi</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.ships}</p>
              </div>
            </div>
          </div>

          <div className="card">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-green-100">
                <Anchor className="h-6 w-6 text-green-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Toplam Liman</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.ports}</p>
              </div>
            </div>
          </div>

          <div className="card">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-purple-100">
                <Calendar className="h-6 w-6 text-purple-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Toplam Ziyaret</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.visits}</p>
              </div>
            </div>
          </div>

          <div className="card">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-orange-100">
                <Package className="h-6 w-6 text-orange-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Toplam Yük</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.cargo}</p>
              </div>
            </div>
          </div>

          <div className="card">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-indigo-100">
                <Users className="h-6 w-6 text-indigo-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Toplam Mürettebat</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.crew}</p>
              </div>
            </div>
          </div>

          <div className="card">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-pink-100">
                <Settings className="h-6 w-6 text-pink-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Toplam Atama</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.assignments}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Quick Actions */}
        <div className="card">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">Hızlı İşlemler</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {quickActions.map((action) => {
              const IconComponent = action.icon;
              return (
                <Link
                  key={action.title}
                  to={action.href}
                  className="block p-4 border border-gray-200 rounded-lg hover:border-gray-300 hover:shadow-sm transition-all duration-200"
                >
                  <div className="flex items-center space-x-3">
                    <div className={`p-2 rounded-lg ${action.color}`}>
                      <IconComponent className="h-5 w-5 text-white" />
                    </div>
                    <div>
                      <h3 className="font-medium text-gray-900">{action.title}</h3>
                      <p className="text-sm text-gray-600">{action.description}</p>
                    </div>
                  </div>
                </Link>
              );
            })}
          </div>
        </div>

        {/* Recent Activity */}
        <div className="card">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-xl font-semibold text-gray-900">Son Aktiviteler</h2>
            <div className="flex items-center space-x-2 text-sm text-gray-500">
              <Activity className="h-4 w-4" />
              <span>Gerçek zamanlı</span>
            </div>
          </div>
          <div className="space-y-3">
            <div className="flex items-center space-x-3 p-3 bg-gray-50 rounded-lg">
              <div className="p-2 rounded-full bg-blue-100">
                <TrendingUp className="h-4 w-4 text-blue-600" />
              </div>
              <div className="flex-1">
                <p className="text-sm font-medium text-gray-900">Sistem aktif</p>
                <p className="text-xs text-gray-500">Tüm servisler çalışıyor</p>
              </div>
              <span className="text-xs text-gray-400">Şimdi</span>
            </div>
            <div className="flex items-center space-x-3 p-3 bg-gray-50 rounded-lg">
              <div className="p-2 rounded-full bg-green-100">
                <Ship className="h-4 w-4 text-green-600" />
              </div>
              <div className="flex-1">
                <p className="text-sm font-medium text-gray-900">Gemi kayıtları</p>
                <p className="text-xs text-gray-500">{stats.ships} gemi sisteme kayıtlı</p>
              </div>
              <span className="text-xs text-gray-400">Güncel</span>
            </div>
            <div className="flex items-center space-x-3 p-3 bg-gray-50 rounded-lg">
              <div className="p-2 rounded-full bg-purple-100">
                <Anchor className="h-4 w-4 text-purple-600" />
              </div>
              <div className="flex-1">
                <p className="text-sm font-medium text-gray-900">Liman kayıtları</p>
                <p className="text-xs text-gray-500">{stats.ports} liman sisteme kayıtlı</p>
              </div>
              <span className="text-xs text-gray-400">Güncel</span>
            </div>
          </div>
        </div>
      </div>
    </MainLayout>
  );
};

export default Dashboard; 