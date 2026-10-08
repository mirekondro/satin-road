import { useCallback, useEffect, useState } from 'react'
import { api } from './client.ts'
import { getErrorMessage } from './errors.ts'
import type { CategoryDto } from './generated/Api.ts'


export function useCategories() {
    const [categories, setCategories] = useState<CategoryDto[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    
    const [version, setVersion] = useState(0)

    useEffect(() => {
        let ignore = false 

        api.categories
            .categoriesList()
            .then((data) => {
                if (ignore) return
                setCategories(data)
                setError(null)
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
    }, [version])

    const reload = useCallback(() => setVersion((v) => v + 1), [])

    return { categories, loading, error, reload }
}