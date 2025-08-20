import React from 'react';
import { Ship, Heart } from 'lucide-react';

const Footer: React.FC = () => {
  return (
    <footer className="bg-white border-t border-gray-200 mt-auto">
      <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div className="flex flex-col md:flex-row justify-between items-center">
          {/* Left side - Brand and description */}
          <div className="flex items-center mb-4 md:mb-0">
            <Ship className="h-6 w-6 text-primary-600 mr-2" />
            <div>
              <h3 className="text-sm font-semibold text-gray-900">Liman Takip Sistemi</h3>
              <p className="text-xs text-gray-500">Gemi ve liman yönetimi için modern çözüm</p>
            </div>
          </div>

          {/* Center - Copyright */}
          <div className="text-center mb-4 md:mb-0">
            <p className="text-sm text-gray-500">
              © 2024 Liman Takip Sistemi. Tüm hakları saklıdır.
            </p>
          </div>

          {/* Right side - Additional links */}
          <div className="flex items-center space-x-4">
            <a
              href="#"
              className="text-xs text-gray-500 hover:text-gray-700 transition-colors duration-200"
            >
              Gizlilik Politikası
            </a>
            <a
              href="#"
              className="text-xs text-gray-500 hover:text-gray-700 transition-colors duration-200"
            >
              Kullanım Şartları
            </a>
            <div className="flex items-center text-xs text-gray-500">
              <span>Made with</span>
              <Heart className="h-3 w-3 text-red-500 mx-1" />
              <span>in Türkiye</span>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer; 