import React, { useState, useEffect } from 'react';
import { X, Save, Calendar, Ship as ShipIcon, User } from 'lucide-react';
import { ShipCrewAssignment, AddShipCrewAssignmentRequest, UpdateShipCrewAssignmentRequest, Ship, CrewMember } from '../../types';

interface ShipCrewAssignmentFormProps {
  assignment?: ShipCrewAssignment | null;
  ships: Ship[];
  crewMembers: CrewMember[];
  onSubmit: (data: AddShipCrewAssignmentRequest | UpdateShipCrewAssignmentRequest) => Promise<void>;
  onCancel: () => void;
}

const ShipCrewAssignmentForm: React.FC<ShipCrewAssignmentFormProps> = ({ 
  assignment, 
  ships,
  crewMembers,
  onSubmit, 
  onCancel 
}) => {
  const [formData, setFormData] = useState<AddShipCrewAssignmentRequest>({
    shipId: 0,
    crewMemberId: 0,
    assignmentDate: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (assignment) {
      setFormData({
        shipId: assignment.shipId,
        crewMemberId: assignment.crewMemberId,
        assignmentDate: assignment.assignmentDate.split('T')[0], // Sadece tarih kısmını al
      });
    } else {
      // Yeni kayıt için bugünün tarihini varsayılan olarak ayarla
      const today = new Date().toISOString().split('T')[0];
      setFormData({
        shipId: 0,
        crewMemberId: 0,
        assignmentDate: today,
      });
    }
  }, [assignment]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.shipId) {
      newErrors.shipId = 'Gemi seçimi zorunludur';
    }

    if (!formData.crewMemberId) {
      newErrors.crewMemberId = 'Mürettebat seçimi zorunludur';
    }

    if (!formData.assignmentDate) {
      newErrors.assignmentDate = 'Atama tarihi zorunludur';
    }

    // Aynı mürettebat üyesinin aynı tarihte başka bir gemiye atanmış olup olmadığını kontrol et
    // Bu kontrol backend'de de yapılmalı, burada sadece client-side validation
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (validateForm()) {
      try {
        // Tarihi ISO string formatına çevir
        const formDataWithDate = {
          ...formData,
          assignmentDate: new Date(formData.assignmentDate).toISOString(),
        };
        
        await onSubmit(formDataWithDate);
      } catch (error: any) {
        if (error.response?.data?.errors) {
          const apiErrors = error.response.data.errors;
          setErrors(prev => ({
            ...prev,
            ...apiErrors
          }));
        } else if (error.message) {
          setErrors(prev => ({
            ...prev,
            general: error.message
          }));
        }
      }
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLSelectElement | HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'shipId' || name === 'crewMemberId' ? parseInt(value) || 0 : value,
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
              {assignment ? 'Atama Düzenle' : 'Yeni Atama Ekle'}
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
                <option key="select-ship" value={0}>Gemi seçiniz</option>
                {ships.map((ship, index) => (
                  <option key={`ship-${ship.shipId || `temp-${index}`}`} value={ship.shipId}>
                    {ship.name} ({ship.imo})
                  </option>
                ))}
              </select>
            </div>
            {errors.shipId && (
              <p className="mt-1 text-sm text-red-600">{errors.shipId}</p>
            )}
          </div>

          {/* CrewMember Selection */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Mürettebat *
            </label>
            <div className="relative">
              <User className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <select
                name="crewMemberId"
                value={formData.crewMemberId}
                onChange={handleChange}
                className={`input-field pl-10 ${errors.crewMemberId ? 'border-red-500' : ''}`}
              >
                <option key="select-crew" value={0}>Mürettebat seçiniz</option>
                {crewMembers.map((crewMember, index) => (
                  <option key={`crew-${crewMember.crewMemberId || `temp-${index}`}`} value={crewMember.crewMemberId}>
                    {crewMember.name} - {crewMember.position}
                  </option>
                ))}
              </select>
            </div>
            {errors.crewMemberId && (
              <p className="mt-1 text-sm text-red-600">{errors.crewMemberId}</p>
            )}
          </div>

          {/* Assignment Date */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Atama Tarihi *
            </label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="date"
                name="assignmentDate"
                value={formData.assignmentDate}
                onChange={handleChange}
                className={`input-field pl-10 ${errors.assignmentDate ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.assignmentDate && (
              <p className="mt-1 text-sm text-red-600">{errors.assignmentDate}</p>
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
              <span>{assignment ? 'Güncelle' : 'Kaydet'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ShipCrewAssignmentForm;
