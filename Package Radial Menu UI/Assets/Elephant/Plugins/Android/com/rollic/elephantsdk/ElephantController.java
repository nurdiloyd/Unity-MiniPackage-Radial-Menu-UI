package com.rollic.elephantsdk;

import android.content.Context;
import android.util.Log;

import com.android.volley.AuthFailureError;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;

import org.json.JSONException;
import org.json.JSONObject;

import java.nio.charset.StandardCharsets;
import java.util.Map;
import java.util.HashMap;

public class ElephantController {

    private static final String LOG_TAG = "[ELEPHANT SDK]";
    private RequestQueue queue;

    private Context ctx;

    private ElephantController(Context ctx) {
        this.ctx = ctx;
        this.queue = Volley.newRequestQueue(this.ctx);
    }


    public static ElephantController create(Context ctx) {
        return new ElephantController(ctx);
    }


    public void ElephantPost(final String url, final String body, final String gameID, final String authToken, int _tryCount) {

        try {
            final int tryCount = _tryCount + 1;
            StringRequest stringRequest = new StringRequest(Request.Method.POST, url,
                    new Response.Listener<String>() {
                        @Override
                        public void onResponse(String response) {
                            // Display the first 500 characters of the response string.
                            Log.e(LOG_TAG, "onResponse: " + response);
                        }
                    }, new Response.ErrorListener() {
                @Override
                public void onErrorResponse(VolleyError error) {

                    try {
                        JSONObject failedReq = new JSONObject();
                        failedReq.accumulate("url", url);
                        failedReq.accumulate("data", body);
                        failedReq.accumulate("tryCount", tryCount);
                        com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant","FailedRequest", failedReq.toString());
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }

                    Log.e(LOG_TAG, "error: " + error.networkResponse);
                }
            }) {

                @Override
                public Map<String, String> getHeaders() throws AuthFailureError {
                    Map<String, String> headers = new HashMap<>();
                    headers.put("Content-Type", "application/json; charset=utf-8");
                    headers.put("Authorization", authToken);
                    headers.put("GameID", gameID);
                    return headers;
                }

                @Override
                public byte[] getBody() throws AuthFailureError {
                    return body.getBytes(StandardCharsets.UTF_8);
                }
            };


            queue.add(stringRequest);

        }catch (Exception e){
            e.printStackTrace();
        }

    }

    public String test() {
        Log.e(LOG_TAG, "test called");

        return "Hello from Elephant android plugin ";
    }


}
