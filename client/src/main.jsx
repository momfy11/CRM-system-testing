import { StrictMode, useState, use, createContext, useEffect } from 'react'
import { createRoot } from 'react-dom/client'
import CreateCustomerView from './components/CreateCustomerView.jsx'
import CustomerTicketView from './components/CustomerTicketView.jsx'
import "./styles.css"
import { BrowserRouter, Routes, Route, useNavigate } from 'react-router'
import { SuperAdminView, SuperAdminCompanyView, SuperAdminAdminView, SuperAdminAddAdminView, SuperAdminEditAdminView, SuperAdminAddCompanyView, SuperAdminEditCompanyView } from "./SuperAdminView.jsx"
import { AdminView, AdminAddProductView, AdminEditProductView, AdminEditSupportView, AdminAddSupportView, ProductView, SupportView, AdminCategoryView } from "./AdminView.jsx"
import { CustomerServiceView, TicketInfoView } from './CustomerService.jsx'
import { ChangePasswordView } from './ChangePasswordView.jsx'
import LoginPage from './LoginPage.jsx'
import TechSolutions from './TechSolutions.jsx'
import EcoEnterprises from './EcoEnterprises.jsx'
import { CustomerChatView } from './components/CustomerChatView.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route index element={<LoginPage />} />
        <Route path='/change-password' element={<ChangePasswordView />} />
        <Route path='/customer' element={<CreateCustomerView />} />
        <Route path='/customer/:slug/chat' element={<CustomerChatView />} />
        <Route path='/customer/addTicket' element={<CustomerTicketView />} />
        <Route path='/super-admin' element={<SuperAdminView />} />
        <Route path='/companies' element={<SuperAdminCompanyView />} />
        <Route path='/companies/add' element={<SuperAdminAddCompanyView />} />
        <Route path='/companies/:id/edit' element={<SuperAdminEditCompanyView />} />
        <Route path='/admins' element={<SuperAdminAdminView />} />
        <Route path='/admins/add' element={<SuperAdminAddAdminView />} />
        <Route path='/customer-service' element={<CustomerServiceView />} />
        <Route path='/customer-service/:slug/ticket-info' element={<TicketInfoView />} />

        <Route path='/categories' element={<AdminCategoryView />} />

        <Route path='/users/:id/edit' element={<SuperAdminEditAdminView />} />

        <Route path='/tech-solutions' element={<TechSolutions />} />
        <Route path='/eco-enterprises' element={<EcoEnterprises />} />

        <Route path='/admin' element={<AdminView />} />
        <Route path='/products' element={<ProductView />} />
        <Route path='/product/add' element={<AdminAddProductView />} />
        <Route path='/product/:id/edit' element={<AdminEditProductView />} />
        <Route path='/agents' element={<SupportView />} />
        <Route path='/agents/:id/edit' element={<AdminEditSupportView />} />
        <Route path='/agents/add' element={<AdminAddSupportView />} />
      </Routes>
    </BrowserRouter>
  </StrictMode>,
);
// adress för create user "http://localhost:5173/create-user"



//console.log(`/api/login/?${document.getElementById("email-field").innerText}&${document.getElementById("password-field").innerText}`)