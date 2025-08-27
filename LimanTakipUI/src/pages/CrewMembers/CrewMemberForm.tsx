import React, { useState, useEffect } from 'react';
import { X, Save, User, Mail, Phone, Briefcase, Globe } from 'lucide-react';
import { CrewMember, AddCrewMemberRequest, UpdateCrewMemberRequest } from '../../types';

interface CrewMemberFormProps {
  crewMember?: CrewMember | null;
  onSubmit: (data: AddCrewMemberRequest | UpdateCrewMemberRequest) => Promise<void>;
  onCancel: () => void;
}

const CrewMemberForm: React.FC<CrewMemberFormProps> = ({ 
  crewMember, 
  onSubmit, 
  onCancel 
}) => {
  const [formData, setFormData] = useState<AddCrewMemberRequest>({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    role: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const commonPositions = [
    'Kaptan',
    'Birinci Zabit',
    'İkinci Zabit',
    'Üçüncü Zabit',
    'Baş Makinist',
    'İkinci Makinist',
    'Üçüncü Makinist',
    'Boatswain',
    'Able Seaman',
    'Ordinary Seaman',
    'Cook',
    'Steward',
    'Radio Officer',
    'Electrician'
  ];



  useEffect(() => {
    if (crewMember) {
      setFormData({
        firstName: crewMember.firstName,
        lastName: crewMember.lastName,
        email: crewMember.email,
        phoneNumber: crewMember.phoneNumber,
        role: crewMember.role ,
      });
    } else {
      setFormData({
        firstName: '',
        lastName: '',
        email: '',
        phoneNumber: '',
        role: '',
      });
    }
  }, [crewMember]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.firstName.trim()) {
      newErrors.firstName = 'İsim  zorunludur';
    }

    if (!formData.lastName.trim()) {
      newErrors.lastName = 'İsim  zorunludur';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'E-posta adresi zorunludur';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Geçerli bir e-posta adresi giriniz';
    }

    if (!formData.phoneNumber.trim()) {
      newErrors.phoneNumber = 'Telefon numarası zorunludur';
    }

    if (!formData.role.trim()) {
      newErrors.role = 'Pozisyon zorunludur';
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
        // Backend'den gelen spesifik hata mesajını kontrol et
        if (error.response?.data?.message) {
          // Backend service layer'dan gelen business logic hataları
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
          // Genel hata mesajı (network hatası vs.)
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
      [name]: value,
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
            <User className="h-6 w-6 text-primary-600" />
            <h2 className="text-xl font-semibold text-gray-900">
              {crewMember ? 'Mürettebat Düzenle' : 'Yeni Mürettebat Ekle'}
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
          {/* FirstName */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              İsim *
            </label>
            <div className="relative">
              <User className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                placeholder="İsim giriniz"
                className={`input-field pl-10 ${errors.firstName ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.firstName && (
              <p className="mt-1 text-sm text-red-600">{errors.firstName}</p>
            )}
          </div>

          {/* LastName */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Soyisim *
            </label>
            <div className="relative">
              <User className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                placeholder="Soyisim  giriniz"
                className={`input-field pl-10 ${errors.lastName ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.lastName && (
              <p className="mt-1 text-sm text-red-600">{errors.lastName}</p>
            )}
          </div>

          {/* Email */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              E-posta *
            </label>
            <div className="relative">
              <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                placeholder="ornek@email.com"
                className={`input-field pl-10 ${errors.email ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.email && (
              <p className="mt-1 text-sm text-red-600">{errors.email}</p>
            )}
          </div>

          {/* Phone */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Telefon *
            </label>
            <div className="relative">
              <Phone className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="tel"
                name="phoneNumber"
                value={formData.phoneNumber}
                onChange={handleChange}
                placeholder="+90 555 123 45 67"
                className={`input-field pl-10 ${errors.phoneNumber ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.phoneNumber && (
              <p className="mt-1 text-sm text-red-600">{errors.phoneNumber}</p>
            )}
          </div>

          {/* Position */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Pozisyon *
            </label>
            <div className="relative">
              <Briefcase className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <select
                name="role"
                value={formData.role}
                onChange={handleChange}
                className={`input-field pl-10 ${errors.role ? 'border-red-500' : ''}`}
              >
                <option key="select-position" value="">Pozisyon seçiniz</option>
                {commonPositions.map(role => (
                  <option key={`position-${role}`} value={role}>{role}</option>
                ))}
              </select>
            </div>
            {errors.role && (
              <p className="mt-1 text-sm text-red-600">{errors.role}</p>
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
              <span>{crewMember ? 'Güncelle' : 'Kaydet'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CrewMemberForm;
