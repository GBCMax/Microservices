import { Navbar } from "./components/navbar";
import { useRoutes } from "./routes/routes";


export function App() {
  const routes = useRoutes()

  return (
    <>
      <Navbar />
      {routes}
    </>
  )
}