import {createApi,fetchBaseQuery} from "@reduxjs/toolkit/query/react";
import { SD_General } from "../constants/constants";
import { updateUrlCodeLinkBody } from "../Interfaces/RequestBody";
import { getAllArgs } from "../Interfaces";

export const urlShortenerApi = createApi({
    reducerPath: "urlShortenerApi",
    baseQuery: fetchBaseQuery({
        baseUrl: "https://localhost:7264/api/",
        prepareHeaders:(headers: Headers, api) => {
            const token = localStorage.getItem(SD_General.tokenKey);
            token && headers.append("Authorization","Bearer " + token); // Pass token so [Authorize] and [Authenticate] can check if user has permission
            headers.append('Access-Control-Allow-Origin', 'http://localhost:3000');
        }
    }),
    tagTypes: ["Urls"],
    endpoints: (builder) => ({
        getAllUrls: builder.query({
            query: (getAllUrlArgs:getAllArgs) => ({
                url: `url/`,
                params: {
                    searchQuery: getAllUrlArgs.searchQuery,
                    pageNumber: getAllUrlArgs.pageNumber,
                    pageSize: getAllUrlArgs.pageSize
                }
            }),
            providesTags: ["Urls"]
       }),
        getUrl: builder.query({
            query: (code: string) => ({
                url: `url/${code}`
            }),
            providesTags: ["Urls"]
       }),
       addUrl: builder.mutation({
            query: (body : any) => ({
                url: "url",
                method: "POST",
                body: body,
                formData: true
            }),
            invalidatesTags: ["Urls"],
       }),
       deleteUrl: builder.mutation({
            query: (code: string) => ({
                url: `url/${code}`,
                method: "DELETE"
            }),
            invalidatesTags: ["Urls"]
       }),
       updateUrlLink: builder.mutation({
        query: (body: updateUrlCodeLinkBody) => ({
            url: `url/${body.code}`,
            method: "PUT",
            params:{
                newUrl: body.newUrl
            }
        }),
        invalidatesTags: ["Urls"]
   }),  
    })
})

export const {useGetAllUrlsQuery,useGetUrlQuery,useAddUrlMutation,useDeleteUrlMutation,useUpdateUrlLinkMutation} = urlShortenerApi;
export default urlShortenerApi;