import { configureStore } from "@reduxjs/toolkit";
import { aliasesApi, urlShortenerApi, userUrlsApi } from "../api";
import authApi from "../api/authApi";
import { userReducer } from "./slices";

const store = configureStore({
    reducer:{
        [urlShortenerApi.reducerPath]: urlShortenerApi.reducer,
        [authApi.reducerPath]: authApi.reducer,
        [aliasesApi.reducerPath]: aliasesApi.reducer,
        [userUrlsApi.reducerPath]: userUrlsApi.reducer,
        userStore: userReducer
    },
    middleware: (getDefaultMiddleware) => getDefaultMiddleware().
    concat(urlShortenerApi.middleware).
    concat(authApi.middleware).
    concat(aliasesApi.middleware).
    concat(userUrlsApi.middleware)
})

export type RootState = ReturnType<typeof store.getState>;

export default store;