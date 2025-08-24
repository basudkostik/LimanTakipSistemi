import axios from 'axios';

// API base configuration
const API_BASE_URL = 'http://localhost:5190/api/v2';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error);
    return Promise.reject(error);
  }
);

// Ship Management API
export const shipAPI = {
  getAll: (params?: any) => api.get('/Ship', {params}),
  getById: (id: number) => api.get(`/Ship/${id}`),
  create: (data: any) => api.post('/Ship', data),
  update: (id: number, data: any) => api.put(`/Ship/${id}`, data),
  delete: (id: number) => api.delete(`/Ship/${id}`),
};

// Port Management API
export const portAPI = {
  getAll: (params?: any) => api.get('/Port', { params }),
  getById: (id: number) => api.get(`/Port/${id}`),
  create: (data: any) => api.post('/Port', data),
  update: (id: number, data: any) => api.put(`/Port/${id}`, data),
  delete: (id: number) => api.delete(`/Port/${id}`),
};

// Ship Visit API
export const shipVisitAPI = {
  getAll: (params?: any) => api.get('/ShipVisit', { params }),
  getById: (id: number) => api.get(`/ShipVisit/${id}`),
  create: (data: any) => api.post('/ShipVisit', data),
  update: (id: number, data: any) => api.put(`/ShipVisit/${id}`, data),
  delete: (id: number) => api.delete(`/ShipVisit/${id}`),
};

// Cargo Management API
export const cargoAPI = {
  getAll: (params?: any) => api.get('/Cargo', { params }),
  getById: (id: number) => api.get(`/Cargo/${id}`),
  create: (data: any) => api.post('/Cargo', data),
  update: (id: number, data: any) => api.put(`/Cargo/${id}`, data),
  delete: (id: number) => api.delete(`/Cargo/${id}`),
};

// Crew Member API
export const crewMemberAPI = {
  getAll: (params?: any) => api.get('/CrewMember', { params }),
  getById: (id: number) => api.get(`/CrewMember/${id}`),
  create: (data: any) => api.post('/CrewMember', data),
  update: (id: number, data: any) => api.put(`/CrewMember/${id}`, data),
  delete: (id: number) => api.delete(`/CrewMember/${id}`),
};

// Ship Crew Assignment API
export const shipCrewAssignmentAPI = {
  getAll: (params?: any) => api.get('/ShipCrewAssignment', { params }),
  getById: (id: number) => api.get(`/ShipCrewAssignment/${id}`),
  create: (data: any) => api.post('/ShipCrewAssignment', data),
  update: (id: number, data: any) => api.put(`/ShipCrewAssignment/${id}`, data),
  delete: (id: number) => api.delete(`/ShipCrewAssignment/${id}`),
};

export default api; 