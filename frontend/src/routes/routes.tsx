import { Route, Routes } from 'react-router-dom';
import { PrivateRoute } from '../components/privateRoute';
import Admin from '../pages/adminPage';
import Logout from '../pages/logoutPage';
import Login from '../pages/loginPage';
import { ProductsPage } from '../pages/productsPage';
import { CarsPage } from '../pages/carsPage';
import { AboutPage } from '../pages/aboutPage';
import Home from '../pages/homePage';

export const useRoutes = () => {

    return (
      <Routes>
        <Route index element={<Home />} />
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        
        <Route element={<PrivateRoute />}>
            <Route path='/products' element={<ProductsPage />} />
            <Route path='/cars' element={<CarsPage />} />
            <Route path='/about' element={<AboutPage />} />
            <Route path='/admin' element={<Admin />} />
            <Route path="/logout" element={<Logout />} />
        </Route>
  
      </Routes>
    )
  }
export default useRoutes