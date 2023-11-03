

import { Auction, AuctionFinished } from '@/types'
import Link from 'next/link'
import Image from 'next/image'
import React from 'react'
import { numberWithCommas } from '../lib/numberWithComma'
type Props = {
    finishedAuction : AuctionFinished
    auction :Auction
}

export default function AuctionFinishedToast({auction,finishedAuction} :Props) {
  return (
    <Link href={`/auctions/details/${auction.id}`} className='flex flex-col items-center'>
        <div className='flex flex-row items-center gap-2'>
            <Image
                  src={auction.imageUrl} alt={'image'} height={80} width={80} className='rounded-lg w-auto h-auto'/>
            <div className='flex flex-col'>
                <span>  Auction For {auction.make } {auction.model} hasFinished </span>    
                {finishedAuction.itemSold && finishedAuction.amount ?(
                    <p> Congrats to {finishedAuction.winner} for winning the bid of $${numberWithCommas(finishedAuction.amount)}</p>
                ):(
                    <p>this item did not sell</p>
                )}
            </div>      
 

        </div>
    </Link>
  )
}
