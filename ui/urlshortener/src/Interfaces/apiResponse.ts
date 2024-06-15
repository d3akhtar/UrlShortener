export default interface apiResponse {
    isSuccess: boolean;
    result?: any;
    statusCode: number;
    redirectUrl?: any;
    errorMessages: string[];
}