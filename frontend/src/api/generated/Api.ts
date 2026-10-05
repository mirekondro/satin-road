/* eslint-disable */
/* tslint:disable */
// @ts-nocheck
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface CategoryDto {
  /** @format int32 */
  id: number;
  name: string;
}

export interface CategoryRequest {
  name: string;
}

export interface ListingRequest {
  /** @format int32 */
  categoryId: number;
  title: string;
  description: null | string;
  /** @format double */
  price: number;
  /** @format int32 */
  stock: number;
}

export interface ListingView {
  /** @format int32 */
  id: number;
  /** @format int32 */
  vendorId: number;
  vendorName: string;
  /** @format int32 */
  categoryId: number;
  categoryName: string;
  title: string;
  description: null | string;
  /** @format double */
  price: number;
  /** @format int32 */
  stock: number;
  isActive: boolean;
  /** @format date-time */
  createdAt: string;
}

export interface ProblemDetails {
  type?: null | string;
  title?: null | string;
  /** @format int32 */
  status?: null | number;
  detail?: null | string;
  instance?: null | string;
}

export interface StockRequest {
  /** @format int32 */
  stock: number;
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseFormat;
  /** request body */
  body?: unknown;
  /** base url */
  baseUrl?: string;
  /** request cancellation token */
  cancelToken?: CancelToken;
}

export type RequestParams = Omit<
  FullRequestParams,
  "body" | "method" | "query" | "path"
>;

export interface ApiConfig<SecurityDataType = unknown> {
  baseUrl?: string;
  baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
  securityWorker?: (
    securityData: SecurityDataType | null,
  ) => Promise<RequestParams | void> | RequestParams | void;
  customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown>
  extends Response {
  data: D;
  error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
  Json = "application/json",
  JsonApi = "application/vnd.api+json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public baseUrl: string = "http://localhost:5273/";
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private abortControllers = new Map<CancelToken, AbortController>();
  private customFetch = (...fetchParams: Parameters<typeof fetch>) =>
    fetch(...fetchParams);

  private baseApiParams: RequestParams = {
    credentials: "same-origin",
    headers: {},
    redirect: "follow",
    referrerPolicy: "no-referrer",
  };

  constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
    Object.assign(this, apiConfig);
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected encodeQueryParam(key: string, value: any) {
    const encodedKey = encodeURIComponent(key);
    return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
  }

  protected addQueryParam(query: QueryParamsType, key: string) {
    return this.encodeQueryParam(key, query[key]);
  }

  protected addArrayQueryParam(query: QueryParamsType, key: string) {
    const value = query[key];
    return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
  }

  protected toQueryString(rawQuery?: QueryParamsType): string {
    const query = rawQuery || {};
    const keys = Object.keys(query).filter(
      (key) => "undefined" !== typeof query[key],
    );
    return keys
      .map((key) =>
        Array.isArray(query[key])
          ? this.addArrayQueryParam(query, key)
          : this.addQueryParam(query, key),
      )
      .join("&");
  }

  protected addQueryParams(rawQuery?: QueryParamsType): string {
    const queryString = this.toQueryString(rawQuery);
    return queryString ? `?${queryString}` : "";
  }

  private contentFormatters: Record<ContentType, (input: any) => any> = {
    [ContentType.Json]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string")
        ? JSON.stringify(input)
        : input,
    [ContentType.JsonApi]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string")
        ? JSON.stringify(input)
        : input,
    [ContentType.Text]: (input: any) =>
      input !== null && typeof input !== "string"
        ? JSON.stringify(input)
        : input,
    [ContentType.FormData]: (input: any) => {
      if (input instanceof FormData) {
        return input;
      }

      return Object.keys(input || {}).reduce((formData, key) => {
        const property = input[key];
        formData.append(
          key,
          property instanceof Blob
            ? property
            : typeof property === "object" && property !== null
              ? JSON.stringify(property)
              : `${property}`,
        );
        return formData;
      }, new FormData());
    },
    [ContentType.UrlEncoded]: (input: any) => this.toQueryString(input),
  };

  protected mergeRequestParams(
    params1: RequestParams,
    params2?: RequestParams,
  ): RequestParams {
    return {
      ...this.baseApiParams,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...(this.baseApiParams.headers || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected createAbortSignal = (
    cancelToken: CancelToken,
  ): AbortSignal | undefined => {
    if (this.abortControllers.has(cancelToken)) {
      const abortController = this.abortControllers.get(cancelToken);
      if (abortController) {
        return abortController.signal;
      }
      return void 0;
    }

    const abortController = new AbortController();
    this.abortControllers.set(cancelToken, abortController);
    return abortController.signal;
  };

  public abortRequest = (cancelToken: CancelToken) => {
    const abortController = this.abortControllers.get(cancelToken);

    if (abortController) {
      abortController.abort();
      this.abortControllers.delete(cancelToken);
    }
  };

  public request = async <T = any, E = any>({
    body,
    secure,
    path,
    type,
    query,
    format,
    baseUrl,
    cancelToken,
    ...params
  }: FullRequestParams): Promise<T> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const queryString = query && this.toQueryString(query);
    const payloadFormatter = this.contentFormatters[type || ContentType.Json];
    const responseFormat = format || requestParams.format;

    return this.customFetch(
      `${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`,
      {
        ...requestParams,
        headers: {
          ...(requestParams.headers || {}),
          ...(type && type !== ContentType.FormData
            ? { "Content-Type": type }
            : {}),
        },
        signal:
          (cancelToken
            ? this.createAbortSignal(cancelToken)
            : requestParams.signal) || null,
        body:
          typeof body === "undefined" || body === null
            ? null
            : payloadFormatter(body),
      },
    ).then(async (response) => {
      const r = response as HttpResponse<T, E>;
      r.data = null as unknown as T;
      r.error = null as unknown as E;

      const responseToParse = responseFormat ? response.clone() : response;
      const data = !responseFormat
        ? r
        : await responseToParse[responseFormat]()
            .then((data) => {
              if (r.ok) {
                r.data = data;
              } else {
                r.error = data;
              }
              return r;
            })
            .catch((e) => {
              r.error = e;
              return r;
            });

      if (cancelToken) {
        this.abortControllers.delete(cancelToken);
      }

      if (!response.ok) throw data;
      return data.data;
    });
  };
}

/**
 * @title SatinRoad.Api | v1
 * @version 1.0.0
 * @baseUrl http://localhost:5273/
 */
export class Api<
  SecurityDataType extends unknown,
> extends HttpClient<SecurityDataType> {
  satinRoadApi = {
    /**
     * No description
     *
     * @tags SatinRoad.Api
     * @name HealthDbList
     * @request GET:/api/health/db
     */
    healthDbList: (params: RequestParams = {}) =>
      this.request<number, any>({
        path: `/api/health/db`,
        method: "GET",
        format: "json",
        ...params,
      }),
  };
  categories = {
    /**
     * No description
     *
     * @tags Categories
     * @name CategoriesList
     * @request GET:/api/categories
     */
    categoriesList: (params: RequestParams = {}) =>
      this.request<CategoryDto[], any>({
        path: `/api/categories`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Categories
     * @name CategoriesCreate
     * @request POST:/api/categories
     */
    categoriesCreate: (data: CategoryRequest, params: RequestParams = {}) =>
      this.request<CategoryDto, ProblemDetails>({
        path: `/api/categories`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Categories
     * @name CategoriesUpdate
     * @request PUT:/api/categories/{id}
     */
    categoriesUpdate: (
      id: number,
      data: CategoryRequest,
      params: RequestParams = {},
    ) =>
      this.request<CategoryDto, ProblemDetails>({
        path: `/api/categories/${id}`,
        method: "PUT",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Categories
     * @name CategoriesDelete
     * @request DELETE:/api/categories/{id}
     */
    categoriesDelete: (id: number, params: RequestParams = {}) =>
      this.request<void, ProblemDetails>({
        path: `/api/categories/${id}`,
        method: "DELETE",
        ...params,
      }),
  };
  listings = {
    /**
     * No description
     *
     * @tags Listings
     * @name ListingsList
     * @request GET:/api/listings
     */
    listingsList: (
      query?: {
        /** @format int32 */
        categoryId?: number;
      },
      params: RequestParams = {},
    ) =>
      this.request<ListingView[], any>({
        path: `/api/listings`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Listings
     * @name ListingsCreate
     * @request POST:/api/listings
     */
    listingsCreate: (data: ListingRequest, params: RequestParams = {}) =>
      this.request<ListingView, ProblemDetails>({
        path: `/api/listings`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Listings
     * @name ListingsDetail
     * @request GET:/api/listings/{id}
     */
    listingsDetail: (id: number, params: RequestParams = {}) =>
      this.request<ListingView, ProblemDetails>({
        path: `/api/listings/${id}`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Listings
     * @name ListingsUpdate
     * @request PUT:/api/listings/{id}
     */
    listingsUpdate: (
      id: number,
      data: ListingRequest,
      params: RequestParams = {},
    ) =>
      this.request<ListingView, ProblemDetails>({
        path: `/api/listings/${id}`,
        method: "PUT",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Listings
     * @name ListingsDelete
     * @request DELETE:/api/listings/{id}
     */
    listingsDelete: (id: number, params: RequestParams = {}) =>
      this.request<void, ProblemDetails>({
        path: `/api/listings/${id}`,
        method: "DELETE",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Listings
     * @name ListingsMineList
     * @request GET:/api/listings/mine
     */
    listingsMineList: (params: RequestParams = {}) =>
      this.request<ListingView[], any>({
        path: `/api/listings/mine`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Listings
     * @name ListingsStockPartialUpdate
     * @request PATCH:/api/listings/{id}/stock
     */
    listingsStockPartialUpdate: (
      id: number,
      data: StockRequest,
      params: RequestParams = {},
    ) =>
      this.request<ListingView, ProblemDetails>({
        path: `/api/listings/${id}/stock`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),
  };
}
