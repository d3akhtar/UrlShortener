import {createApi,fetchBaseQuery} from "@reduxjs/toolkit/query/react";
import { SD_General } from "../constants/constants";
import { updateAliasBody } from "../Interfaces/RequestBody";
import { getAllArgs } from "../Interfaces";

export const aliasesApi = createApi({
    reducerPath: "aliasesApi",
    baseQuery: fetchBaseQuery({
        baseUrl: "https://localhost:7264/api/",
        prepareHeaders:(headers: Headers, api) => {
            const token = localStorage.getItem(SD_General.tokenKey);
            token && headers.append("Authorization","Bearer " + token); // Pass token so [Authorize] and [Authenticate] can check if user has permission
        }
    }),
    tagTypes: ["Aliases"],
    endpoints: (builder) => ({
        getAllAliases: builder.query({
            query: (getAllUrlArgs:getAllArgs) => ({
                url: `alias/`,
                params: {
                    searchQuery: getAllUrlArgs.searchQuery,
                    pageNumber: getAllUrlArgs.pageNumber,
                    pageSize: getAllUrlArgs.pageSize
                }
            }),
            providesTags: ["Aliases"]
       }),
       addAlias: builder.mutation({
            query: (body : any) => ({
                url: "alias",
                method: "POST",
                body : body,
            }),
            invalidatesTags: ["Aliases"]
       }),
       updateAlias: builder.mutation({
            query: (body : updateAliasBody) => ({
                url: `alias/${body.code}`,
                method: "PUT",
                headers: {
                    "Content-type": "application/json"
                },
                body: {
                    code: body.newCode,
                    url: body.url
                },
            }),
            invalidatesTags: ["Aliases"],
       }),
       deleteAlias: builder.mutation({
            query: (code:string) => ({
                url: `alias/${code}`,
                method: "DELETE"
            }),
            invalidatesTags: ["Aliases"],
       }) 
    })
})

export const {useGetAllAliasesQuery,useAddAliasMutation,useUpdateAliasMutation,useDeleteAliasMutation} = aliasesApi;
export default aliasesApi;