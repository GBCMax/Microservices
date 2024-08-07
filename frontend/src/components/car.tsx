import { useState } from "react";
import { ICar } from "../models/car";

interface CarProps {
    car: ICar
}

export function Car({car}: CarProps){
    const [details, setDetails] = useState(false)

    const btnBgClassName = details ? 'bg-yellow-400' : 'bg-blue-400'

    const btnClasses = ['py-2 px-4 border', btnBgClassName]

    return (
        <div
            className="border py-2 px-4 rounded flex flex-col items-center mb-2"
        >
            <p>{ car.name }</p>
            <p className="font-bold">{ car.description }</p>
            <button 
                className={btnClasses.join(' ')}
                onClick={() => setDetails(!details)}
            >{ details ? 'Hide details' : 'Show details' }
            </button>
            {details && <div>
                <p>{car.description}</p>
            </div>}
        </div>
    )
}