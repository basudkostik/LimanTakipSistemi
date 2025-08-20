import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import ShipList from './pages/Ships/ShipList';
import PortList from './pages/Ports/PortList';
import ShipVisitList from './pages/ShipVisits/ShipVisitList';
import CargoList from './pages/Cargo/CargoList';
import CrewMemberList from './pages/CrewMembers/CrewMemberList';
import ShipCrewAssignmentList from './pages/ShipCrewAssignments/ShipCrewAssignmentList';
import Dashboard from './pages/Dashboard/Dashboard';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/ships" element={<ShipList />} />
        <Route path="/ports" element={<PortList />} />
        <Route path="/visits" element={<ShipVisitList />} />
        <Route path="/cargo" element={<CargoList />} />
        <Route path="/crew" element={<CrewMemberList />} />
        <Route path="/assignments" element={<ShipCrewAssignmentList />} />
      </Routes>
    </Router>
  );
}

export default App; 