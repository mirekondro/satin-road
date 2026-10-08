import { Api } from './generated/Api.ts'
import { getStoredToken } from '../auth/authStorage.ts'


export const api = new Api({

    baseUrl: '',
    
    baseApiParams: { secure: true },
    
    securityWorker: () => {
        const token = getStoredToken()
        return token ? { headers: { Authorization: `Bearer ${token}` } } : {}
    },
})