// Ship Management
export interface Ship {
  shipId: number;
  name: string;
  imo: string;
  type: string;
  flag: string;
  yearBuilt: number;
}

export interface AddShipRequest {
  name: string;
  imo: string;
  type: string;
  flag: string;
  yearBuilt: number;
}

export interface UpdateShipRequest {
  name: string;
  imo: string;
  type: string;
  flag: string;
  yearBuilt: number;
}

// Port Management
export interface Port {
  portId: number;
  name: string;
  country: string;
  city: string;
}

export interface AddPortRequest {
  name: string;
  country: string;
  city: string;
}

export interface UpdatePortRequest {
  name: string;
  country: string;
  city: string;
}

// Ship Visit Management
export interface ShipVisit {
  visitId: number;
  shipId: number;
  portId: number;
  arrivalDate: string;
  departureDate: string;
  purpose: string;
  ship?: Ship;
  port?: Port;
}

export interface AddShipVisitRequest {
  shipId: number;
  portId: number;
  arrivalDate: string;
  departureDate: string;
  purpose: string;
}

export interface UpdateShipVisitRequest {
  shipId: number;
  portId: number;
  arrivalDate: string;
  departureDate: string;
  purpose: string;
}

// Cargo Management
export interface Cargo {
  cargoId: number;
  shipId: number;
  description: string;
  weight: number;
  cargoType: string;
  ship?: Ship;
}

export interface AddCargoRequest {
  shipId: number;
  description: string;
  weight: number;
  cargoType: string;
}

export interface UpdateCargoRequest {
  shipId: number;
  description: string;
  weight: number;
  cargoType: string;
}

// Crew Member Management
export interface CrewMember {
  crewMemberId: number;
  name: string;
  email: string;
  phone: string;
  position: string;
  nationality: string;
}

export interface AddCrewMemberRequest {
  name: string;
  email: string;
  phone: string;
  position: string;
  nationality: string;
}

export interface UpdateCrewMemberRequest {
  name: string;
  email: string;
  phone: string;
  position: string;
  nationality: string;
}

// Ship Crew Assignment Management
export interface ShipCrewAssignment {
  assignmentId: number;
  shipId: number;
  crewMemberId: number;
  assignmentDate: string;
  ship?: Ship;
  crewMember?: CrewMember;
}

export interface AddShipCrewAssignmentRequest {
  shipId: number;
  crewMemberId: number;
  assignmentDate: string;
}

export interface UpdateShipCrewAssignmentRequest {
  shipId: number;
  crewMemberId: number;
  assignmentDate: string;
}

// Common Types
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginationParams {
  page: number;
  pageSize: number;
} 