import {createApi,fetchBaseQuery} from "@reduxjs/toolkit/query/react";
import { SD_General } from "../constants/constants";
import { addUrlBody, deleteUrlBody } from "../Interfaces/RequestBody";
import { getAllArgs } from "../Interfaces";

export const userUrlsApi = createApi({
    reducerPath: "userUrlsApi",
    baseQuery: fetchBaseQuery({
        baseUrl: "https://localhost:7264/api/",
        prepareHeaders:(headers: Headers, api) => {
            const token = localStorage.getItem(SD_General.tokenKey);
            token && headers.append("Authorization","Bearer " + token); // Pass token so [Authorize] and [Authenticate] can check if user has permission
        }
    }),
    tagTypes: ["UserUrls"],
    endpoints: (builder) => ({
        getAllUserUrls: builder.query({
            query: () => ({
                url: `ApplicationUserUrls/`
            }),
            providesTags: ["UserUrls"]
       }),
       getUserUrls: builder.query({
            query: (getAllArgs:getAllArgs) => ({
                url: `ApplicationUserUrls/${getAllArgs.userId}`,
                params:{
                    searchQuery: getAllArgs.searchQuery,
                    pageNumber: getAllArgs.pageNumber,
                    pageSize: getAllArgs.pageSize
                }
            }),
            providesTags: ["UserUrls"]
       }),
       addUserUrl: builder.mutation({
            query: (body : addUrlBody) => ({
                url: `ApplicationUserUrls/${body.userId}`,
                method: "POST",
                headers: {
                    "Content-type": "application/json"
                },
                body: body.codes
            }),
            invalidatesTags: ["UserUrls"]
       }),
       deleteUserUrl: builder.mutation({
            query: (body : deleteUrlBody) => ({
                url: `ApplicationUserUrls/${body.userId}`,
                method: "DELETE",
                params: {
                    code: body.code
                }
            }),
            invalidatesTags: ["UserUrls"],
       }) 
    })
})

export const {useGetAllUserUrlsQuery,useGetUserUrlsQuery,useAddUserUrlMutation,useDeleteUserUrlMutation} = userUrlsApi;
export default userUrlsApi;