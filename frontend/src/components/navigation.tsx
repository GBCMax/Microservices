import { Link } from "react-router-dom";

export function Navigation() {
    return (
        <nav className="h-[50px] flex justify-between px-5 bg-gray-500 items-center text-white">
            <span className="font-bold">React 2024</span>
            <span>
                <Link to="/" className="mr-2">Products</Link>
                <Link to="/cars" className="mr-4">Cars</Link>
                <Link to="/about">About</Link>
            </span>
        </nav>
    )
}