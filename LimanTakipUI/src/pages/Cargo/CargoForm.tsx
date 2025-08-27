import React, { useState, useEffect } from 'react';
import { X, Save, Package, Ship as ShipIcon } from 'lucide-react';
import { Cargo, AddCargoRequest, UpdateCargoRequest, Ship } from '../../types';

interface CargoFormProps {
  cargo?: Cargo | null;
  ships: Ship[];
  onSubmit: (data: AddCargoRequest | UpdateCargoRequest) => Promise<void>;
  onCancel: () => void;
}

const CargoForm: React.FC<CargoFormProps> = ({ 
  cargo, 
  ships, 
  onSubmit, 
  onCancel 
}) => {
  const [formData, setFormData] = useState<AddCargoRequest>({
    shipId: 0,
    description: '',
    weight: 0,
    cargoType: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (cargo) {
      setFormData({
        shipId: cargo.shipId,
        description: cargo.description,
        weight: cargo.weight,
        cargoType: cargo.cargoType,
      });
    } else {
      setFormData({
        shipId: 0,
        description: '',
        weight: 0,
        cargoType: '',
      });
    }
  }, [cargo]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.shipId) {
      newErrors.shipId = 'Gemi seçimi zorunludur';
    }

    if (!formData.description.trim()) {
      newErrors.description = 'Yük açıklaması zorunludur';
    }

    if (!formData.weight || formData.weight <= 0) {
      newErrors.weight = 'Geçerli bir ağırlık giriniz';
    }

    if (!formData.cargoType.trim()) {
      newErrors.cargoType = 'Yük tipi zorunludur';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (validateForm()) {
      try {
        await onSubmit(formData);
      } catch (error: any) {
        
        if (error.response?.data?.message) {
          //  service layer hataları
          setErrors(prev => ({
            ...prev,
            general: error.response.data.message
          }));
        } else if (error.response?.data?.errors) {
          // Model validation hataları
          const apiErrors = error.response.data.errors;
          setErrors(prev => ({
            ...prev,
            ...apiErrors
          }));
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

  const handleChange = (e: React.ChangeEvent<HTMLSelectElement | HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'shipId' ? parseInt(value) || 0 : 
              name === 'weight' ? parseFloat(value) || 0 : value,
    }));
    
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <div className="flex items-center space-x-3">
            <Package className="h-6 w-6 text-primary-600" />
            <h2 className="text-xl font-semibold text-gray-900">
              {cargo ? 'Yük Düzenle' : 'Yeni Yük Ekle'}
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
          {/* Ship Selection */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Gemi *
            </label>
            <div className="relative">
              <ShipIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <select
                name="shipId"
                value={formData.shipId}
                onChange={handleChange}
                className={`input-field pl-10 ${errors.shipId ? 'border-red-500' : ''}`}
              >
                <option value={0}>Gemi seçiniz</option>
                {ships.map(ship => (
                  <option key={ship.shipId} value={ship.shipId}>
                    {ship.name} ({ship.imo})
                  </option>
                ))}
              </select>
            </div>
            {errors.shipId && (
              <p className="mt-1 text-sm text-red-600">{errors.shipId}</p>
            )}
          </div>

          {/* Cargo Type */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Yük Tipi *
            </label>
            <div className="relative">
              <Package className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                name="cargoType"
                value={formData.cargoType}
                onChange={handleChange}
                placeholder="Örn: Konteyner, Dökme Yük, Sıvı Yük..."
                className={`input-field pl-10 ${errors.cargoType ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.cargoType && (
              <p className="mt-1 text-sm text-red-600">{errors.cargoType}</p>
            )}
          </div>

          {/* Weight */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Ağırlık (ton) *
            </label>
            <div className="relative">
              <input
                type="number"
                name="weight"
                value={formData.weight}
                onChange={handleChange}
                min="0"
                step="0.01"
                placeholder="Yük ağırlığını giriniz"
                className={`input-field ${errors.weight ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.weight && (
              <p className="mt-1 text-sm text-red-600">{errors.weight}</p>
            )}
          </div>

          {/* Description */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Açıklama *
            </label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleChange}
              placeholder="Yük hakkında detaylı açıklama giriniz..."
              rows={3}
              className={`input-field ${errors.description ? 'border-red-500' : ''}`}
            />
            {errors.description && (
              <p className="mt-1 text-sm text-red-600">{errors.description}</p>
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
              <span>{cargo ? 'Güncelle' : 'Kaydet'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CargoForm;
