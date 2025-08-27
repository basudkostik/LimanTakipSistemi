
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


export interface CrewMember {
  crewId: number;
  firstName: string;
  lastName:string;
  email: string;
  phoneNumber: string;
  role: string;
}

export interface AddCrewMemberRequest {
 firstName: string;
  lastName:string;
  email: string;
  phoneNumber: string;
  role: string;
}

export interface UpdateCrewMemberRequest {
 firstName: string;
  lastName:string;
  email: string;
  phoneNumber: string;
  role: string;
}

 
export interface ShipCrewAssignment {
  assignmentId: number;
  shipId: number;
  crewId: number;
  assignmentDate: string;
  ship? : Ship;
  crewMember? : CrewMember;
}

export interface AddShipCrewAssignmentRequest {
  shipId: number;
  crewId: number;
  assignmentDate: string;
}

export interface UpdateShipCrewAssignmentRequest {
  shipId: number;
  crewId: number;
  assignmentDate: string;
}

 
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginationParams {
  page: number;
  pageSize: number;
} 