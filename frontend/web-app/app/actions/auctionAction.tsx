'use server'
import { Auction, PageResult } from "@/types";
import { getTokenWorkAround } from "./authActions";

export async function getData(query:string) :Promise<PageResult<Auction>> {
    console.log(query)
    const res = await fetch(`http://localhost:6001/search${query}`);
    if(!res.ok){
        throw new Error('Failed to fetch');
    }
    const data = await res.json();
    return data;
} 

export async function UpdateAuctionTest(){
    debugger
    const data ={
        mileage:Math.floor(Math.random()*10000)+1
    }
    console.log(data)
    const token = await getTokenWorkAround()
    const res = await fetch(`http://localhost:6001/auctions/6a5011a1-fe1f-47df-9a32-b5346b289391`,{
        method:'PUT',
        headers:{
            'Content-type':'application/json',
            'Authorization':`Bearer ${token?.access_token}`
        },
        body:JSON.stringify(data)
        
    })
  
    if (!res.ok) return {status:res.status,message : res.statusText}
    return res.statusText
}