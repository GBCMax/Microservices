import useAuth from '../hooks/useAuth';
import { useLocation, useNavigate } from 'react-router-dom';

const Login = () => {
  const { setAuth } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const from = location.state?.from?.pathname || '/'

  return (
    <>
        <div className="
            py-2 
            px-4 
            flex 
            flex-col 
            items-center 
            place-content-center 
            h-dvh 
            bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500
            text-violet-800">
            <input 
                placeholder='Email' 
                type="text"
                className="border py-2 px-4 mb-6 w-1/5 outline-0 shadow-xl self-center"
            />
            <input 
                placeholder='Password'
                type="password"
                className="border py-2 px-4 mb-6 w-1/5 outline-0 shadow-xl self-center"
            />
            <button
            type="submit" 
            className="py-2 px-4 bg-yellow-400 flex flex-col hover:text-white" 
            onClick={() => {
                setAuth(true)
                navigate(from, { replace: true });
            }}
        >Login</button>
        </div>
    </>
  )
}

export default Login