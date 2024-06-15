import { SD_General } from "../constants/constants";

const withAuth = (WrappedComponent : any) => {
    return(props: any) => {
        if (!localStorage.getItem(SD_General.tokenKey)){
            window.location.replace("/UrlShortener/Login")
            return null; 
        }
        return <WrappedComponent {...props}/>
    }
}

export default withAuth;