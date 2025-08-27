import React, { useState, useEffect } from 'react';
import { X, Save, Calendar, Ship as ShipIcon, Anchor } from 'lucide-react';
import { ShipVisit, AddShipVisitRequest, UpdateShipVisitRequest, Ship, Port } from '../../types';

interface ShipVisitFormProps {
  shipVisit?: ShipVisit | null;
  ships: Ship[];
  ports: Port[];
  onSubmit: (data: AddShipVisitRequest | UpdateShipVisitRequest) => Promise<void>;
  onCancel: () => void;
}

const ShipVisitForm: React.FC<ShipVisitFormProps> = ({ 
  shipVisit, 
  ships, 
  ports, 
  onSubmit, 
  onCancel 
}) => {
  const [formData, setFormData] = useState<AddShipVisitRequest>({
    shipId: 0,
    portId: 0,
    arrivalDate: '',
    departureDate: '',
    purpose: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (shipVisit) {
      setFormData({
        shipId: shipVisit.shipId,
        portId: shipVisit.portId,
        arrivalDate: shipVisit.arrivalDate.split('T')[0], // Sadece tarih kısmını al
        departureDate: shipVisit.departureDate.split('T')[0],
        purpose: shipVisit.purpose || '',
      });
    } else {
      // Yeni kayıt için bugünün tarihini varsayılan olarak ayarla
      const today = new Date().toISOString().split('T')[0];
      setFormData({
        shipId: 0,
        portId: 0,
        arrivalDate: today,
        departureDate: today,
        purpose: '',
      });
    }
  }, [shipVisit]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.shipId) {
      newErrors.shipId = 'Gemi seçimi zorunludur';
    }

    if (!formData.portId) {
      newErrors.portId = 'Liman seçimi zorunludur';
    }

    if (!formData.arrivalDate) {
      newErrors.arrivalDate = 'Varış tarihi zorunludur';
    }

    if (!formData.departureDate) {
      newErrors.departureDate = 'Ayrılış tarihi zorunludur';
    }

    if (formData.arrivalDate && formData.departureDate) {
      const arrival = new Date(formData.arrivalDate);
      const departure = new Date(formData.departureDate);
      
      if (arrival > departure) {
        newErrors.departureDate = 'Ayrılış tarihi varış tarihinden sonra olmalıdır';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (validateForm()) {
      try {
        
        const formDataWithDates = {
          ...formData,
          arrivalDate: new Date(formData.arrivalDate).toISOString(),
          departureDate: new Date(formData.departureDate).toISOString(),
        };
        
        await onSubmit(formDataWithDates);
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
      [name]: name === 'shipId' || name === 'portId' ? parseInt(value) || 0 : value,
    }));
    
    // Clear error when user starts typing
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
            <Calendar className="h-6 w-6 text-primary-600" />
            <h2 className="text-xl font-semibold text-gray-900">
              {shipVisit ? 'Ziyaret Düzenle' : 'Yeni Ziyaret Ekle'}
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

          {/* Port Selection */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Liman *
            </label>
            <div className="relative">
              <Anchor className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <select
                name="portId"
                value={formData.portId}
                onChange={handleChange}
                className={`input-field pl-10 ${errors.portId ? 'border-red-500' : ''}`}
              >
                <option value={0}>Liman seçiniz</option>
                {ports.map(port => (
                  <option key={port.portId} value={port.portId}>
                    {port.name} - {port.city}, {port.country}
                  </option>
                ))}
              </select>
            </div>
            {errors.portId && (
              <p className="mt-1 text-sm text-red-600">{errors.portId}</p>
            )}
          </div>

          {/* Arrival Date */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Varış Tarihi *
            </label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="date"
                name="arrivalDate"
                value={formData.arrivalDate}
                onChange={handleChange}
                className={`input-field pl-10 ${errors.arrivalDate ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.arrivalDate && (
              <p className="mt-1 text-sm text-red-600">{errors.arrivalDate}</p>
            )}
          </div>

          {/* Departure Date */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Ayrılış Tarihi *
            </label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="date"
                name="departureDate"
                value={formData.departureDate}
                onChange={handleChange}
                className={`input-field pl-10 ${errors.departureDate ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.departureDate && (
              <p className="mt-1 text-sm text-red-600">{errors.departureDate}</p>
            )}
          </div>

          {/* Purpose */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Ziyaret Amacı
            </label>
            <textarea
              name="purpose"
              value={formData.purpose}
              onChange={handleChange}
              placeholder="Ziyaret amacını giriniz..."
              rows={3}
              className={`input-field ${errors.purpose ? 'border-red-500' : ''}`}
            />
            {errors.purpose && (
              <p className="mt-1 text-sm text-red-600">{errors.purpose}</p>
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
              <span>{shipVisit ? 'Güncelle' : 'Kaydet'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ShipVisitForm;
