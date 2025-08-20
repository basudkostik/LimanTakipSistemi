import React from 'react';
import Header from './Header';
import Footer from './Footer';
import Sidebar from './Sidebar';

interface MainLayoutProps {
  children: React.ReactNode;
  filters?: React.ReactNode;
  sidebarContent?: React.ReactNode;
  sidebarTitle?: string;
  showSidebar?: boolean;
}

const MainLayout: React.FC<MainLayoutProps> = ({ 
  children, 
  filters, 
  sidebarContent,
  sidebarTitle, 
  showSidebar = true 
}) => {
  // Use sidebarContent if provided, otherwise fall back to filters
  const sidebarFilters = sidebarContent || filters;
  
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      {/* Header */}
      <Header />
      
      {/* Main Content Area */}
      <div className="flex-1 flex">
        {showSidebar ? (
          <Sidebar filters={sidebarFilters} title={sidebarTitle}>
            <div className="h-full overflow-y-auto p-6">
              {children}
            </div>
          </Sidebar>
        ) : (
          <div className="flex-1 overflow-y-auto p-6">
            {children}
          </div>
        )}
      </div>
      
      {/* Footer */}
      <Footer />
    </div>
  );
};

export default MainLayout; 