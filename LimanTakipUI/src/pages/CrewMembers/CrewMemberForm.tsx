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
    name: '',
    email: '',
    phone: '',
    position: '',
    nationality: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  // Yaygın pozisyonlar
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

  // Yaygın ülkeler
  const commonCountries = [
    'Türkiye',
    'Philippines',
    'India',
    'Ukraine',
    'Russia',
    'Romania',
    'Bulgaria',
    'Poland',
    'Croatia',
    'Greece',
    'Myanmar',
    'China',
    'Indonesia',
    'Vietnam'
  ];

  useEffect(() => {
    if (crewMember) {
      setFormData({
        name: crewMember.name,
        email: crewMember.email,
        phone: crewMember.phone,
        position: crewMember.position,
        nationality: crewMember.nationality,
      });
    } else {
      setFormData({
        name: '',
        email: '',
        phone: '',
        position: '',
        nationality: '',
      });
    }
  }, [crewMember]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'İsim soyisim zorunludur';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'E-posta adresi zorunludur';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Geçerli bir e-posta adresi giriniz';
    }

    if (!formData.phone.trim()) {
      newErrors.phone = 'Telefon numarası zorunludur';
    }

    if (!formData.position.trim()) {
      newErrors.position = 'Pozisyon zorunludur';
    }

    if (!formData.nationality.trim()) {
      newErrors.nationality = 'Uyrukluk zorunludur';
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
          {/* Name */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              İsim Soyisim *
            </label>
            <div className="relative">
              <User className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                name="name"
                value={formData.name}
                onChange={handleChange}
                placeholder="İsim ve soyisim giriniz"
                className={`input-field pl-10 ${errors.name ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.name && (
              <p className="mt-1 text-sm text-red-600">{errors.name}</p>
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
                name="phone"
                value={formData.phone}
                onChange={handleChange}
                placeholder="+90 555 123 45 67"
                className={`input-field pl-10 ${errors.phone ? 'border-red-500' : ''}`}
              />
            </div>
            {errors.phone && (
              <p className="mt-1 text-sm text-red-600">{errors.phone}</p>
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
                name="position"
                value={formData.position}
                onChange={handleChange}
                className={`input-field pl-10 ${errors.position ? 'border-red-500' : ''}`}
              >
                <option key="select-position" value="">Pozisyon seçiniz</option>
                {commonPositions.map(position => (
                  <option key={`position-${position}`} value={position}>{position}</option>
                ))}
              </select>
            </div>
            {errors.position && (
              <p className="mt-1 text-sm text-red-600">{errors.position}</p>
            )}
          </div>

          {/* Nationality */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Uyrukluk *
            </label>
            <div className="relative">
              <Globe className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <select
                name="nationality"
                value={formData.nationality}
                onChange={handleChange}
                className={`input-field pl-10 ${errors.nationality ? 'border-red-500' : ''}`}
              >
                <option key="select-nationality" value="">Uyrukluk seçiniz</option>
                {commonCountries.map(country => (
                  <option key={`nationality-${country}`} value={country}>{country}</option>
                ))}
              </select>
            </div>
            {errors.nationality && (
              <p className="mt-1 text-sm text-red-600">{errors.nationality}</p>
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
