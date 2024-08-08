import { Link } from 'react-router-dom'
import useAuth from '../hooks/useAuth'

export function Navbar() {
  const { isAuthenticated } = useAuth()
  return (
    <>
      <nav className="h-[50px] flex justify-between px-5 bg-gray-500 items-center text-white">
        <Link to="/">Home</Link>
        {isAuthenticated && <Link to="/products" className="mr-2">Products</Link>}
        {isAuthenticated && <Link to="/cars" className="mr-4">Cars</Link>}
        {isAuthenticated && <Link to="/about" className="mr-6">About</Link>}
        {isAuthenticated && <Link to="/admin">Admin</Link>}
        {isAuthenticated ? <Link to="/logout">Logout</Link> : <Link to="/login">Login</Link>}
      </nav>
    </>
  )
}