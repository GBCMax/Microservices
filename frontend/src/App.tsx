import {Route, Routes} from "react-router-dom"
import { ProductsPage } from "./pages/productsPage";
import { AboutPage } from "./pages/aboutPage";
import { Navigation } from "./components/navigation";
import { CarsPage } from "./pages/carsPage";

function App() {
  return (
    <>
    <Navigation/>
      <Routes>
        <Route path="/" element={<ProductsPage/>}/>
        <Route path="/cars" element={<CarsPage/>}/>
        <Route path="/about" element={<AboutPage/>}/>
      </Routes>
    </>
  )
}

export default App;
