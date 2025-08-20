import React from 'react';
import { Ship, Anchor, Users, Package, Calendar, Settings, Home } from 'lucide-react';
import { Link, useLocation } from 'react-router-dom';

const Header: React.FC = () => {
  const location = useLocation();

  const navigation = [
    { name: 'Dashboard', href: '/dashboard', icon: Home, current: location.pathname === '/dashboard' },
    { name: 'Gemi Yönetimi', href: '/ships', icon: Ship, current: location.pathname.startsWith('/ships') },
    { name: 'Liman Yönetimi', href: '/ports', icon: Anchor, current: location.pathname.startsWith('/ports') },
    { name: 'Ziyaret Kayıtları', href: '/visits', icon: Calendar, current: location.pathname.startsWith('/visits') },
    { name: 'Yük Yönetimi', href: '/cargo', icon: Package, current: location.pathname.startsWith('/cargo') },
    { name: 'Mürettebat Yönetimi', href: '/crew', icon: Users, current: location.pathname.startsWith('/crew') },
    { name: 'Gemi-Mürettebat Atamaları', href: '/assignments', icon: Settings, current: location.pathname.startsWith('/assignments') },
  ];

  return (
    <header className="bg-white shadow-sm border-b border-gray-200">
      <div className="max-w-7xl mx-auto px-0 sm:px-0 lg:px-0">
        <div className="flex justify-between items-center h-16">
          {/* Logo and Brand */}
          <div className="flex items-center">
            <div className="flex-shrink-0">
              <div className="flex items-center">
                <Ship className="h-8 w-8 text-primary-600" />
                <span className="ml-0 text-xl font-bold text-gray-900">Liman Takip Sistemi</span>
              </div>
            </div>
          </div>

          {/* Navigation */}
          <nav className="hidden md:flex space-x-8">
            {navigation.map((item) => {
              const Icon = item.icon;
              return (
                <Link
                  key={item.name}
                  to={item.href}
                  className={`inline-flex items-center px-1 pt-1 text-sm font-medium border-b-2 transition-colors duration-200 ${
                    item.current
                      ? 'border-primary-500 text-primary-600'
                      : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  }`}
                >
                  <Icon className="mr-2 h-4 w-4" />
                  {item.name}
                </Link>
              );
            })}
          </nav>

          {/* Mobile menu button */}
          <div className="md:hidden">
            <button
              type="button"
              className="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-primary-500"
            >
              <span className="sr-only">Open main menu</span>
              <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>
      </div>

      {/* Mobile Navigation */}
      <div className="md:hidden">
        <div className="px-2 pt-2 pb-3 space-y-1 sm:px-3 bg-gray-50">
          {navigation.map((item) => {
            const Icon = item.icon;
            return (
              <Link
                key={item.name}
                to={item.href}
                className={`flex items-center px-3 py-2 text-base font-medium rounded-md transition-colors duration-200 ${
                  item.current
                    ? 'bg-primary-100 text-primary-700 border-l-4 border-primary-500'
                    : 'text-gray-600 hover:text-gray-800 hover:bg-gray-100'
                }`}
              >
                <Icon className="mr-3 h-5 w-5" />
                {item.name}
              </Link>
            );
          })}
        </div>
      </div>
    </header>
  );
};

export default Header; 