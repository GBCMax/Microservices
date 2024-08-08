import useAuth from '../hooks/useAuth';
import { useLocation, useNavigate } from 'react-router-dom';

const Login = () => {
  const { setAuth } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const from = location.state?.from?.pathname || '/'

  return (
    <>
        <div>
            <input 
                placeholder='Email' 
                type="text"
                className="border py-2 px-4 mb-2 w-full outline-0"
            />
            <input 
                placeholder='Password'
                type="text"
                className="border py-2 px-4 mb-2 w-full outline-0"
            />
        </div>
        <button
            type="submit" 
            className="py-2 px-4 border bg-yellow-400 hover:text-white" 
            onClick={() => {
                setAuth(true)
                navigate(from, { replace: true });
            }}
        >Login</button>
    </>
  )
}

export default Login