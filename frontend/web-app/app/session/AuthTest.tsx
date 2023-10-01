'use client'
import React from 'react'
import { UpdateAuctionTest } from '../actions/auctionAction';
import { Button } from 'flowbite-react';

export default function AuthTest() {
    const [Loading ,setLoading] = React.useState(false)
    const [result, setResult] = React.useState<any>();
    function doUpdate(){
        setLoading(true);
        setResult(undefined);
        UpdateAuctionTest().then(res => {
        
            setResult(res)
        }).finally(() => setLoading(false))
    }
  return (
    <div className='flex items-center gap-4 '>
        <Button outline isProcessing={Loading} onClick={doUpdate} >
            Test Auth
        </Button>
        <div>
            {JSON.stringify(result,null,2)}
        </div>
    </div>
  )
}
