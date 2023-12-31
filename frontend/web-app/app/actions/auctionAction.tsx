'use server'
import { Auction, Bid, PageResult } from "@/types";
import { getTokenWorkAround } from "./authActions";
import { fetchWrapper } from "@/lib/fetchWrapper";
import { FieldValues } from "react-hook-form";
import { revalidatePath } from "next/cache";

export async function getData(query:string) :Promise<PageResult<Auction>> {
    await fetchWrapper.get(`search${query}`).then(x => console.log(x));
    return await fetchWrapper.get(`search${query}`);
} 

export async function UpdateAuctionTest(){
 
    const data ={
        mileage:Math.floor(Math.random()*10000)+1
    }

   return await fetchWrapper.put(`auctions/6a5011a1-fe1f-47df-9a32-b5346b289391`,data)
}
export async function createAuction(data:FieldValues){
    return await fetchWrapper.post('auctions',data)
}

export async function getDetailedViewData(id:string) :Promise<Auction>{
    return await fetchWrapper.get('auctions/'+id)
}

export async function updateAuction(data:FieldValues ,id:string) :Promise<Auction>{
    const res= await fetchWrapper.put( `auctions/${id}`,data)
    revalidatePath(`/auctions/${id}`)
    return res;
}
export async function deleteAuction(id:string) {
    return await fetchWrapper.del(`auctions/${id}`)
}

export async function getBidsForAuction (id:string) :Promise<Bid[]>{
   
    return await fetchWrapper.get(`bids/${id}`)
}

export async function placeBidForAuction (auctionId :string, amount :number){
    return await fetchWrapper.post(`bids?auctionId=${auctionId}&amount=${amount}`,{})
}