import { Link } from "react-router-dom";
import useAuth from "../hooks/useAuth";

export function Navigation() {
    const { isAuthenticated } = useAuth()
    return (
        <nav className="h-[50px] flex justify-between px-5 bg-gray-500 items-center text-white">
            <span className="font-bold">React 2024</span>
            <span>
                <Link to="/">Main</Link>
                <Link to="/products" className="mr-2">Products</Link>
                <Link to="/cars" className="mr-4">Cars</Link>
                <Link to="/about" className="mr-6">About</Link>
                {isAuthenticated && <Link to="/admin" className="mr-8">Admin</Link>}
                {isAuthenticated ? <Link to="/logout" className="mr-10">Logout</Link> : <Link to="/login">Login</Link>}
            </span>
        </nav>
    )
}