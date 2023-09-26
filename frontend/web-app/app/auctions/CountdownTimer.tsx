import { type } from 'os';
import React from 'react'
import Countdown, { zeroPad } from 'react-countdown';
type Pros={
    auctionEnd : string;
}
const renderer = ({days, hours, minutes, seconds, completed }:{days:number,hours:number,minutes:number,seconds:number,completed:boolean}) => {
    return (
        <div className={`
        border-2 border-white text-white py-1 rounded-lg flex justify-center p-2
        ${completed? 
            'bg-red-600': (days===0 && hours<10)?
            'bg-amber-600': 'bg-green-600'}`}>
                {completed ? (<span>Auction Finished</span>) : (
                    <span suppressHydrationWarning={true}>
                        {zeroPad(days) }:{zeroPad(hours)  }:{zeroPad(minutes) }:{zeroPad(seconds) }
                    </span>
                )}
        </div>
    )
  };

export default function CountdownTimer({auctionEnd} :Pros) {
  return (
    <div>
        <Countdown date={auctionEnd} renderer={renderer}></Countdown>
    </div>
  )
}
