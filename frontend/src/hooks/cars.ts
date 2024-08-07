import { useEffect, useState } from "react";
import { ICar } from "../models/car";
import axios, { AxiosError } from "axios";
import { ILogin } from "../models/loginRequest";
import { useCookies } from "react-cookie";

export function UseCars() {

    const loginRequest: ILogin = {
        email: 'bob@le-magnifique.com',
        password: '}s>EWG@f4g;_v7nB'
    }

    const token = {
        token: 'bob@le-magnifique.com'
    }

    const  [ cookies ,  setCookie ,  removeCookie ]  =  useCookies ( [ 'token' ] ) ;
    const [cars, setCars] = useState<ICar[]>([])
    const [error, setError] = useState('')
    const [loading, setLoading] = useState(false)

    function addCar(car: ICar) {
        setCars(c => [...c, car])
    }

    async function fetchCars() {
        try{
            setError('')
            setLoading(true)
            const loginResponse = await axios.post('https://localhost:7186/api/Authentication/Login', loginRequest)
            console.log("token")
            console.log(loginResponse.data)
            setCookie('token', loginResponse.data)
            token.token = loginResponse.data
            //axios.post('your_url', data, {withCredentials: true});
            const response = await axios.post<ICar[]>('https://localhost:7186/api/Car/CarList', token)
            // const response = await fetch('https://localhost:7186/api/Authentication/Login', {

            //     method: 'GET',
        
            //     headers: new Headers({'Cookie': loginResponse.data })
        
            // });
            console.log(response)
            setCars(response.data)
            setLoading(false)
        }
        catch(e: unknown){
            const error = e as AxiosError
            setLoading(false)
            setError(error.message)
        }
    }

    useEffect(() => {
        fetchCars()
    }, [])

    return {cars, error, loading, addCar}
}