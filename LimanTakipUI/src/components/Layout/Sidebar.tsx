import React, { useState } from 'react';
import { Filter, Search, X, ChevronDown, ChevronRight } from 'lucide-react';

interface SidebarProps {
  children: React.ReactNode;
  filters?: React.ReactNode;
  title?: string;
}

const Sidebar: React.FC<SidebarProps> = ({ children, filters, title }) => {
  const [isOpen, setIsOpen] = useState(true);

  return (
    <div className="flex h-full">
      {/* Sidebar */}
      <div className={`${isOpen ? 'w-80' : 'w-16'} bg-white border-r border-gray-200 transition-all duration-300 ease-in-out flex-shrink-0`}>
        <div className="flex flex-col h-full">
          {/* Sidebar Header */}
          <div className="flex items-center justify-between p-4 border-b border-gray-200">
            <h2 className={`font-semibold text-gray-900 ${!isOpen && 'hidden'}`}>
              {title || 'Filtreler'}
            </h2>
            <button
              onClick={() => setIsOpen(!isOpen)}
              className="p-1 rounded-md text-gray-400 hover:text-gray-600 hover:bg-gray-100"
            >
              {isOpen ? <ChevronRight className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
            </button>
          </div>

          {/* Sidebar Content */}
          <div className="flex-1 overflow-y-auto">
            {isOpen && (
              <div className="p-4">
                {filters || (
                  <div className="space-y-4">
                    <div className="flex items-center space-x-2">
                      <Filter className="h-4 w-4 text-gray-500" />
                      <span className="text-sm font-medium text-gray-700">Filtreler</span>
                    </div>
                    
                    {/* Search Filter */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Arama
                      </label>
                      <div className="relative">
                        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <input
                          type="text"
                          placeholder="Ara..."
                          className="input-field pl-10"
                        />
                      </div>
                    </div>

                    {/* Date Range Filter */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Tarih Aralığı
                      </label>
                      <div className="space-y-2">
                        <input
                          type="date"
                          className="input-field"
                        />
                        <input
                          type="date"
                          className="input-field"
                        />
                      </div>
                    </div>

                    {/* Status Filter */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Durum
                      </label>
                      <select className="input-field">
                        <option value="">Tümü</option>
                        <option value="active">Aktif</option>
                        <option value="inactive">Pasif</option>
                      </select>
                    </div>

                    {/* Clear Filters Button */}
                    <button className="w-full btn-secondary text-sm">
                      Filtreleri Temizle
                    </button>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="flex-1 overflow-hidden">
        {children}
      </div>
    </div>
  );
};

export default Sidebar; 