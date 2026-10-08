import { useEffect, useState } from 'react'
import { api } from './client.ts'
import { getErrorMessage } from './errors.ts'
import type { FeaturedVendor } from './generated/Api.ts'

// Hard story #13 – vendors with more than 100 sales
export function useFeaturedVendors() {
    const [vendors, setVendors] = useState<FeaturedVendor[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        let ignore = false

        api.vendors
            .vendorsFeaturedList()
            .then((data) => {
                if (!ignore) setVendors(data)
            })
            .catch((e) => {
                if (!ignore) setError(getErrorMessage(e))
            })
            .finally(() => {
                if (!ignore) setLoading(false)
            })

        return () => {
            ignore = true
        }
    }, [])

    return { vendors, loading, error }
}
