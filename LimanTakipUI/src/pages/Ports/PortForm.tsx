import React, { useState, useEffect } from 'react';
import { X, Save, Anchor } from 'lucide-react';
import { Port as PortType, AddPortRequest, UpdatePortRequest } from '../../types';

interface PortFormProps {
  port?: PortType | null;
  onSubmit: (data: AddPortRequest | UpdatePortRequest) => void;
  onCancel: () => void;
}

const PortForm: React.FC<PortFormProps> = ({ port, onSubmit, onCancel }) => {
  const [formData, setFormData] = useState<AddPortRequest>({
    name: '',
    country: '',
    city: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (port) {
      setFormData({
        name: port.name,
        country: port.country,
        city: port.city,
      });
    }
  }, [port]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Liman adı zorunludur';
    }

    if (!formData.country.trim()) {
      newErrors.country = 'Ülke zorunludur';
    }

    if (!formData.city.trim()) {
      newErrors.city = 'Şehir zorunludur';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validateForm()) {
      if (port) {
        onSubmit({ ...formData, portId: port.portId });
      } else {
        onSubmit(formData);
      }
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value,
    }));
    
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const countries = [
    'Türkiye',
    'Almanya',
    'Fransa',
    'İtalya',
    'İspanya',
    'Hollanda',
    'Belçika',
    'Yunanistan',
    'Bulgaristan',
    'Romanya',
    'Ukrayna',
    'Rusya',
    'Gürcistan',
    'Azerbaycan',
    'İran',
    'Irak',
    'Suriye',
    'Lübnan',
    'İsrail',
    'Mısır',
    'Libya',
    'Tunus',
    'Cezayir',
    'Fas',
    'İngiltere',
    'İrlanda',
    'Norveç',
    'İsveç',
    'Finlandiya',
    'Danimarka',
    'Polonya',
    'Çek Cumhuriyeti',
    'Slovakya',
    'Macaristan',
    'Avusturya',
    'İsviçre',
    'Portekiz',
    'Diğer'
  ];

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-md mx-4">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <div className="flex items-center space-x-3">
            <Anchor className="h-6 w-6 text-primary-600" />
            <h2 className="text-xl font-semibold text-gray-900">
              {port ? 'Liman Düzenle' : 'Yeni Liman Ekle'}
            </h2>
          </div>
          <button
            onClick={onCancel}
            className="text-gray-400 hover:text-gray-600 transition-colors"
          >
            <X className="h-6 w-6" />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          {/* Name Field */}
          <div>
            <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-2">
              Liman Adı *
            </label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleChange}
              className={`input-field ${errors.name ? 'border-red-500' : ''}`}
              placeholder="İstanbul Limanı"
            />
            {errors.name && (
              <p className="mt-1 text-sm text-red-600">{errors.name}</p>
            )}
          </div>

          {/* Country Field */}
          <div>
            <label htmlFor="country" className="block text-sm font-medium text-gray-700 mb-2">
              Ülke *
            </label>
            <select
              id="country"
              name="country"
              value={formData.country}
              onChange={handleChange}
              className={`input-field ${errors.country ? 'border-red-500' : ''}`}
            >
              <option value="">Ülke seçin</option>
              {countries.map(country => (
                <option key={country} value={country}>{country}</option>
              ))}
            </select>
            {errors.country && (
              <p className="mt-1 text-sm text-red-600">{errors.country}</p>
            )}
          </div>

          {/* City Field */}
          <div>
            <label htmlFor="city" className="block text-sm font-medium text-gray-700 mb-2">
              Şehir *
            </label>
            <input
              type="text"
              id="city"
              name="city"
              value={formData.city}
              onChange={handleChange}
              className={`input-field ${errors.city ? 'border-red-500' : ''}`}
              placeholder="İstanbul"
            />
            {errors.city && (
              <p className="mt-1 text-sm text-red-600">{errors.city}</p>
            )}
          </div>

          {/* Action Buttons */}
          <div className="flex space-x-3 pt-4">
            <button
              type="button"
              onClick={onCancel}
              className="flex-1 btn-secondary"
            >
              İptal
            </button>
            <button
              type="submit"
              className="flex-1 btn-primary flex items-center justify-center space-x-2"
            >
              <Save className="h-4 w-4" />
              <span>{port ? 'Güncelle' : 'Kaydet'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default PortForm; 