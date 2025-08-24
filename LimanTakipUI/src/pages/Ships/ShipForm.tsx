import React, { useState, useEffect } from 'react';
import { X, Save, Ship } from 'lucide-react';
import { Ship as ShipType, AddShipRequest, UpdateShipRequest } from '../../types';

interface ShipFormProps {
  ship?: ShipType | null;
  onSubmit: (data: AddShipRequest | UpdateShipRequest) => Promise<void>;
  onCancel: () => void;
}

const ShipForm: React.FC<ShipFormProps> = ({ ship, onSubmit, onCancel }) => {
  const [formData, setFormData] = useState<AddShipRequest>({
    name: '',
    imo: '',
    type: '',
    flag: '',
    yearBuilt: new Date().getFullYear(),
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (ship) {
      setFormData({
        name: ship.name,
        imo: ship.imo,
        type: ship.type,
        flag: ship.flag,
        yearBuilt: ship.yearBuilt,
      });
    }
  }, [ship]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Gemi adı zorunludur';
    }

    if (!formData.imo.trim()) {
      newErrors.imo = 'IMO numarası zorunludur';
    } else if (formData.imo.length < 7) {
      newErrors.imo = 'IMO numarası en az 7 karakter olmalıdır';
    }

    if (!formData.type.trim()) {
      newErrors.type = 'Gemi tipi zorunludur';
    }

    if (!formData.flag.trim()) {
      newErrors.flag = 'Bayrak zorunludur';
    }

    if (formData.yearBuilt < 1900 || formData.yearBuilt > new Date().getFullYear() + 1) {
      newErrors.yearBuilt = 'Geçerli bir yapım yılı giriniz';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();

  if (validateForm()) {
    try {
    
      const formDataWithIMO = {
        ...formData,
        imo: formData.imo.startsWith('IMO') ? formData.imo : `IMO${formData.imo}`
      };
      await onSubmit(formDataWithIMO);
    } catch (error: any) {
      if (error.response?.data?.errors) {
        const apiErrors = error.response.data.errors;
        
        
        if (apiErrors.imo && apiErrors.imo.includes('unique')) {
          setErrors(prev => ({
            ...prev,
            imo: 'IMO numarası benzersiz olmalı'
          }));
        } else {
          setErrors(prev => ({
            ...prev,
            ...apiErrors
          }));
        }
      } else if (error.message) {
        // Genel hata mesajı
        setErrors(prev => ({
          ...prev,
          general: error.message
        }));
      }
    }
  }
};

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'yearBuilt' ? parseInt(value) || 0 : value,
    }));
    
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const shipTypes = [
    'Konteyner Gemisi',
    'Tanker',
    'Bulk Carrier',
    'Ro-Ro Gemisi',
    'Yolcu Gemisi',
    'Balıkçı Gemisi',
    'Yat',
    'Diğer'
  ];



  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <div className="flex items-center space-x-3">
            <Ship className="h-6 w-6 text-primary-600" />
            <h2 className="text-xl font-semibold text-gray-900">
              {ship ? 'Gemi Düzenle' : 'Yeni Gemi Ekle'}
            </h2>
          </div>
          <button
            onClick={onCancel}
            className="text-gray-400 hover:text-gray-600 p-1"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          {/* Ship Name */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Gemi Adı *
            </label>
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              className={`input-field ${errors.name ? 'border-red-500' : ''}`}
              placeholder="Gemi adını giriniz"
            />
            {errors.name && (
              <p className="mt-1 text-sm text-red-600">{errors.name}</p>
            )}
          </div>

          {/* IMO Number */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              IMO Numarası *
            </label>
            <input
              type="text"
              name="imo"
              value={formData.imo}
              onChange={handleChange}
              className={`input-field ${errors.imo ? 'border-red-500' : ''}`}
              placeholder="IMO numarasını giriniz"
            />
            {errors.imo && (
              <p className="mt-1 text-sm text-red-600">{errors.imo}</p>
            )}
          </div>

          {/* Ship Type */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Gemi Tipi *
            </label>
            <select
              name="type"
              value={formData.type}
              onChange={handleChange}
              className={`input-field ${errors.type ? 'border-red-500' : ''}`}
            >
              <option value="">Gemi tipi seçiniz</option>
              {shipTypes.map(type => (
                <option key={type} value={type}>{type}</option>
              ))}
            </select>
            {errors.type && (
              <p className="mt-1 text-sm text-red-600">{errors.type}</p>
            )}
          </div>

          {/* Flag */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Bayrak *
            </label>
            <input
              type='text'
              name="flag"
              value={formData.flag}
              onChange={handleChange}
              className={`input-field ${errors.flag ? 'border-red-500' : ''}`}
              placeholder='Bayrak giriniz'
             />
          
            {errors.flag && (
              <p className="mt-1 text-sm text-red-600">{errors.flag}</p>
            )}
          </div>

          {/* Year Built */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Yapım Yılı *
            </label>
            <input
              type="number"
              name="yearBuilt"
              value={formData.yearBuilt}
              onChange={handleChange}
              min="1900"
              max={new Date().getFullYear() + 1}
              className={`input-field ${errors.yearBuilt ? 'border-red-500' : ''}`}
              placeholder="Yapım yılını giriniz"
            />
            {errors.yearBuilt && (
              <p className="mt-1 text-sm text-red-600">{errors.yearBuilt}</p>
            )}
          </div>

          {/* General Error */}
          {errors.general && (
            <div className="bg-red-50 border border-red-200 rounded-md p-3">
              <p className="text-sm text-red-600">{errors.general}</p>
            </div>
          )}

          {/* Form Actions */}
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
              <span>{ship ? 'Güncelle' : 'Kaydet'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ShipForm; 