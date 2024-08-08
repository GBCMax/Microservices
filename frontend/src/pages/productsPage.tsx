import { useContext } from "react"
import { CreateProduct } from "../components/createProduct"
import { ErrorMessage } from "../components/errorMessage"
import { Loader } from "../components/loader"
import { Modal } from "../components/modal"
import { Product } from "../components/product"
import { ModalContext } from "../context/modalContext"
import { useProducts } from "../hooks/products"
import { IProduct } from "../models/product"

export function ProductsPage() {
    const {loading, error, products, addProduct} = useProducts()
  const {modal, open, close} = useContext(ModalContext)

  const createHandler = (product: IProduct) => {
    close()
    addProduct(product)
  }

  return(
    <div className='container mx-auto max-w-2xl pt-5'>
      { loading &&<Loader/> }
      { error &&<ErrorMessage error={error}/> }
      { products.map(p => <Product product={p} key={p.id}/>) }

      { modal && 
      <Modal title='Create new product' onClose={() => close()}>
        <CreateProduct onCreate={createHandler}/>
      </Modal>
      }

      <button 
        className='fixed bottom-5 right-5 rounded-full bg-red-700 text-white text-2xl px-4 py-2'
        onClick={() => open()}
      >
        +
      </button>
    </div>
  )
}