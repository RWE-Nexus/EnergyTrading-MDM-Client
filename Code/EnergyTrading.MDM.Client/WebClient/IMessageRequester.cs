namespace EnergyTrading.Mdm.Client.WebClient
{
    using System.Collections.Generic;

    using EnergyTrading.Contracts.Search;

    /// <summary>
    /// Retrieves information over a REST API.
    /// </summary>
    public interface IMessageRequester
    {
        /// <summary>
        /// Create a message at a uri
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="uri"></param>
        /// <param name="message"></param>
        /// <returns>Response code and the location of the new entity if successful</returns>
        WebResponse<TMessage> Create<TMessage>(string uri, TMessage message);

        /// <summary>
        /// Delete a message at a uri
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="uri"></param>
        /// <returns>Response code
        /// </returns>
        WebResponse<TMessage> Delete<TMessage>(string uri);

        /// <summary>
        /// Request information from a uri
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        WebResponse<TMessage> Request<TMessage>(string uri);

        /// <summary>
        /// Search for contracts on uri 
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="uri"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        PagedWebResponse<IList<TContract>> Search<TContract>(string uri, Search message);

        /// <summary>
        /// Update entity on uri
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="uri"></param>
        /// <param name="etag"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        WebResponse<TMessage> Update<TMessage>(string uri, string etag, TMessage message);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="message"></param>
        /// <param name="requestInfo"></param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        WebResponse<TMessage> Create<TMessage>(string uri, TMessage message, MdmRequestInfo requestInfo);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestInfo"></param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        WebResponse<TMessage> Delete<TMessage>(string uri, MdmRequestInfo requestInfo);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="etag"></param>
        /// <param name="message"></param>
        /// <param name="requestInfo"></param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        WebResponse<TMessage> Update<TMessage>(string uri, string etag, TMessage message, MdmRequestInfo requestInfo);
    }
}