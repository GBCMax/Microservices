import { Ref, useState } from 'react';
import useAuth from '../hooks/useAuth';
import { useLocation, useNavigate } from 'react-router-dom';
import { ILogin } from '../models/loginRequest';
import axios from 'axios';
import { useCookies } from 'react-cookie';
import { ModalError } from '../context/modalError';

const loginRequest: ILogin = {
    email: '',
    password: ''
}

const Login = () => {
    
    const  [ cookies ,  setCookie ,  removeCookie ]  =  useCookies ( [ 'token' ] ) ;
    const { setAuth } = useAuth()
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate()
    const location = useLocation()
    const from = location.state?.from?.pathname || '/';
    const [showErrorModal, setShowModalError] = useState(false);
    const showErrorNotification = () => {
        setShowModalError(true);
      }
    
      const closeModalError = () => {
        setShowModalError(false)
      }
    const handleEmailChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(event.target.value);
    }

    const handlePasswordChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(event.target.value);
    }

    async function Login() {
        loginRequest.email = email;
        loginRequest.password = password;
        try{
            const loginResponse = await axios.post('https://localhost:7186/api/Authentication/Login', loginRequest);
            setCookie('token', loginResponse.data);
            setAuth(true)
            navigate(from, { replace: true });
            console.log(loginResponse.data);
        }
        catch(e){
            showErrorNotification();
            setEmail('');
            setPassword('');
        }
    }   

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
                    value={email}
                    onChange={handleEmailChange}
                    className="border py-2 px-4 mb-6 w-1/5 outline-0 shadow-xl self-center"
                />
                <input 
                    placeholder='Password'
                    type="password"
                    value={password}
                    onChange={handlePasswordChange}
                    className="border py-2 px-4 mb-6 w-1/5 outline-0 shadow-xl self-center"
                />
                <button
                type="submit" 
                className="py-2 px-4 mb-6 bg-yellow-400 flex flex-col hover:text-white" 
                onClick={async() => {
                    console.log(loginRequest.email);
                    console.log(loginRequest.password);
                    await Login();
                }}
                >Login</button>
                <button
                    type='submit'
                    className='py-2 px-4 bg-blue-400 flex flex-col hover:text-white'
                >
                    Register
                </button>
                {showErrorModal && <ModalError message='Неверный логин или пароль' onClose={closeModalError} />}
            </div>
        </>
    )
}

export default Login