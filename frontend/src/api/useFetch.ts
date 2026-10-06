import { useCallback, useEffect, useState } from 'react'
import { getErrorMessage } from './errors.ts'

interface FetchState<T> {
    data?: T
    error?: string
    fetcher?: () => Promise<T>
    version?: number
}


export function useFetch<T>(fetcher: () => Promise<T>) {
    const [state, setState] = useState<FetchState<T>>({})
    const [version, setVersion] = useState(0)

    useEffect(() => {
        let ignore = false
        fetcher()
            .then((data) => {
                if (!ignore) setState({ data, fetcher, version })
            })
            .catch((e) => {
                if (!ignore) setState({ error: getErrorMessage(e), fetcher, version })
            })
        return () => {
            ignore = true
        }
    }, [fetcher, version])

    const reload = useCallback(() => setVersion((v) => v + 1), [])

    const loading = state.fetcher !== fetcher || state.version !== version

    return { data: state.data, error: loading ? undefined : state.error, loading, reload }
}