import { Car } from "../components/car"
import { ErrorMessage } from "../components/errorMessage"
import { Loader } from "../components/loader"
import { UseCars } from "../hooks/cars"
import { ICar } from "../models/car"

export function CarsPage() {
    const {loading, error, cars} = UseCars()

    // const createHandler = (car: ICar) => {
    //     addCar(car)
    // }

    return (
        <div className='container mx-auto max-w-2xl pt-5'>
            {loading && <Loader/>}
            {error && <ErrorMessage error={error}/>}
            {cars.map(c => <Car car = {c} key={c.id}/>)}
        </div>
    )
}