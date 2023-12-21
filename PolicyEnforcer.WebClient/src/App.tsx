import { Routes, Route } from 'react-router-dom';
import LoginPage from './pages/login.page';
import AdminPage from './pages/admin.page';
import BrowserHistory from './components/Users/BrowserHistory';
import Hardware from './components/Users/Hardware';
import MainLayout from './components/Layout/MainLayout';

function App() {
  return (
    <Routes>
      <Route path="/" element={<MainLayout />}>
        <Route path="/admin" element={<AdminPage />} />
        <Route path="/browser-history/:userId" element={<BrowserHistory />} />
        <Route path="/hardware/:userId" element={<Hardware />} />
      </Route>

      <Route path="/login" element={<LoginPage />} />
   
    </Routes>
  );
}

export default App;
