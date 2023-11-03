'use client'
import { placeBidForAuction } from '@/app/actions/auctionAction';
import { numberWithCommas } from '@/app/lib/numberWithComma';
import { useBidStore } from '@/hooks/useBidStore';
import { stat } from 'fs';
import { type } from 'os'
import React from 'react'
import { FieldValue, FieldValues, useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
type Props = {
    auctionId:string;
    highBid:number;
}
export default function BidForm({auctionId,highBid}:Props) {
    const {register,handleSubmit,reset,formState:{errors}}=useForm();

    const addBid =useBidStore(state => state.addBid);
   
    function onSubmit (data : FieldValues){
        if(data.amount<=highBid) {
            reset();
            return toast.error(`Bid must be higher than $${numberWithCommas(highBid+1)}`)
        }
        placeBidForAuction(auctionId,data.amount).then(bid =>{
            if(bid.error) throw bid.error
            addBid(bid);
            reset();
        }).catch(err => toast.error(err.message))
    }
  return (
    <form onSubmit={handleSubmit(onSubmit)} className='flex items-center border-2 rounded-lg py-2'>
        <input type='number' 
        {...register('amount')}
        className='input-custom text-sm text-gray-600' 
        placeholder={`Enter Your Bid (min bid is $${numberWithCommas(highBid+1)})`}></input>
    </form>
  )
}
