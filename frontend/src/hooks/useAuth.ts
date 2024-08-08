import { useContext } from "react"
import AuthContext from "../context/authProvider"

function useAuth() {
  return (
    useContext(AuthContext)
  )
}

export default useAuth